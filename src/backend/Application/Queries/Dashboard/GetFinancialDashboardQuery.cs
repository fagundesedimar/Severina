using MediatR;
using Severina.Application.DTOs;
using Severina.Domain.Enums;
using Severina.Domain.Interfaces;
using Severina.Application.Interfaces;

namespace Severina.Application.Queries.Dashboard;

public record GetFinancialDashboardQuery(Guid CompanyId) : IRequest<FinancialDashboardResponse>;

public class GetFinancialDashboardQueryHandler : IRequestHandler<GetFinancialDashboardQuery, FinancialDashboardResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFinancialCacheService _cacheService;

    public GetFinancialDashboardQueryHandler(IUnitOfWork unitOfWork, IFinancialCacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<FinancialDashboardResponse> Handle(GetFinancialDashboardQuery request, CancellationToken cancellationToken)
    {
        var cached = await _cacheService.GetDashboardAsync<FinancialDashboardResponse>(request.CompanyId);
        if (cached != null)
            return cached;

        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1);
        var startOfNextMonth = startOfMonth.AddMonths(1);
        var endOfNextMonth = startOfNextMonth.AddMonths(1).AddTicks(-1);

        var receitasMes = await _unitOfWork.Transactions.GetTotalApprovedByTypeAsync(
            request.CompanyId, TransactionType.Receita, startOfMonth, endOfMonth);
        var despesasMes = await _unitOfWork.Transactions.GetTotalApprovedByTypeAsync(
            request.CompanyId, TransactionType.Despesa, startOfMonth, endOfMonth);

        var receitasProximoMes = await _unitOfWork.Transactions.GetTotalApprovedByTypeAsync(
            request.CompanyId, TransactionType.Receita, startOfNextMonth, endOfNextMonth);
        var despesasProximoMes = await _unitOfWork.Transactions.GetTotalApprovedByTypeAsync(
            request.CompanyId, TransactionType.Despesa, startOfNextMonth, endOfNextMonth);

        var allTimeReceitas = await _unitOfWork.Transactions.GetTotalApprovedByTypeAsync(
            request.CompanyId, TransactionType.Receita, DateTime.MinValue, now);
        var allTimeDespesas = await _unitOfWork.Transactions.GetTotalApprovedByTypeAsync(
            request.CompanyId, TransactionType.Despesa, DateTime.MinValue, now);

        var pendingCount = await _unitOfWork.Invoices.CountPagedAsync(request.CompanyId, InvoiceStatus.Pending);
        var overdueCount = await _unitOfWork.Invoices.CountPagedAsync(request.CompanyId, InvoiceStatus.Overdue);

        var monthlyData = await GetMonthlyDataAsync(request.CompanyId, now);
        var categoryData = await GetCategoryDataAsync(request.CompanyId, now);

        var recentTransactions = await _unitOfWork.Transactions.GetPagedAsync(request.CompanyId, 1, 10);
        var recentResponse = recentTransactions.Select(t => new TransactionResponse(
            t.Id, t.CompanyId, t.ClientId, t.Tipo, t.Valor, t.Data,
            t.Categoria, t.Descricao, t.Status, t.CreatedAt, t.UpdatedAt)).ToList();

        var kpis = new FinancialKpisDto(
            allTimeReceitas - allTimeDespesas,
            receitasMes,
            despesasMes,
            receitasProximoMes - despesasProximoMes,
            pendingCount,
            overdueCount);

        var charts = new FinancialChartsDto(monthlyData, categoryData);
        var response = new FinancialDashboardResponse(kpis, charts, recentResponse);

        await _cacheService.SetDashboardAsync(request.CompanyId, response);

        return response;
    }

    private async Task<IReadOnlyList<MonthlyDataDto>> GetMonthlyDataAsync(Guid companyId, DateTime now)
    {
        var result = new List<MonthlyDataDto>();

        for (int i = 11; i >= 0; i--)
        {
            var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(-i);
            var monthEnd = monthStart.AddMonths(1).AddTicks(-1);

            var receitas = await _unitOfWork.Transactions.GetTotalApprovedByTypeAsync(
                companyId, TransactionType.Receita, monthStart, monthEnd);
            var despesas = await _unitOfWork.Transactions.GetTotalApprovedByTypeAsync(
                companyId, TransactionType.Despesa, monthStart, monthEnd);

            result.Add(new MonthlyDataDto(monthStart.ToString("MMM"), receitas, despesas));
        }

        return result;
    }

    private async Task<IReadOnlyList<CategoryDataDto>> GetCategoryDataAsync(Guid companyId, DateTime now)
    {
        var startOfYear = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var result = new List<CategoryDataDto>();

        foreach (TransactionCategory category in Enum.GetValues<TransactionCategory>())
        {
            var transactions = await _unitOfWork.Transactions.GetPagedAsync(
                companyId, 1, int.MaxValue, TransactionType.Despesa, category, startOfYear, now);
            var total = transactions.Where(t => t.Status == TransactionStatus.Approved).Sum(t => t.Valor);

            if (total > 0)
            {
                result.Add(new CategoryDataDto(category.ToString(), total, 0));
            }
        }

        var grandTotal = result.Sum(c => c.Valor);
        if (grandTotal > 0)
        {
            result = result.Select(c => c with { Percentual = Math.Round(c.Valor / grandTotal * 100, 1) }).ToList();
        }

        return result.OrderByDescending(c => c.Valor).ToList();
    }
}
