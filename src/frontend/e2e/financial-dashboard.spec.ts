import { test, expect } from '@playwright/test';

test.describe('Financial Dashboard', () => {
  test('dashboard page renders KPI cards', async ({ page }) => {
    await page.goto('/financeiro');

    await expect(page.locator('h1')).toContainText('Financeiro');
    await expect(page.locator('text=Saldo')).toBeVisible();
    await expect(page.locator('text=Receitas do Mês')).toBeVisible();
    await expect(page.locator('text=Despesas do Mês')).toBeVisible();
    await expect(page.locator('text=Contas Pendentes')).toBeVisible();
  });

  test('dashboard shows monthly chart', async ({ page }) => {
    await page.goto('/financeiro');

    await expect(page.locator('text=Receitas vs Despesas')).toBeVisible();
  });

  test('dashboard shows category chart', async ({ page }) => {
    await page.goto('/financeiro');

    await expect(page.locator('text=Despesas por Categoria')).toBeVisible();
  });

  test('dashboard shows recent transactions', async ({ page }) => {
    await page.goto('/financeiro');

    await expect(page.locator('text=Transações Recentes')).toBeVisible();
  });

  test('dashboard has link to all transactions', async ({ page }) => {
    await page.goto('/financeiro');

    const link = page.locator('a:has-text("Ver todas")');
    await expect(link).toBeVisible();
    await link.click();
    await expect(page).toHaveURL(/.*transacoes/);
  });
});
