import { test, expect } from '@playwright/test';

test.describe('Transactions Page', () => {
  test('transactions page renders', async ({ page }) => {
    await page.goto('/financeiro/transacoes');

    await expect(page.locator('h1')).toContainText('Transações');
    await expect(page.locator('button:has-text("Nova Transação")')).toBeVisible();
    await expect(page.locator('button:has-text("Exportar")')).toBeVisible();
  });

  test('create transaction form shows on button click', async ({ page }) => {
    await page.goto('/financeiro/transacoes');

    await page.locator('button:has-text("Nova Transação")').click();

    await expect(page.locator('select').first()).toBeVisible();
    await expect(page.locator('input[type="number"]')).toBeVisible();
    await expect(page.locator('input[type="date"]')).toBeVisible();
  });

  test('create transaction form hides on cancel', async ({ page }) => {
    await page.goto('/financeiro/transacoes');

    await page.locator('button:has-text("Nova Transação")').click();
    await expect(page.locator('input[type="number"]')).toBeVisible();

    await page.locator('button:has-text("Cancelar")').click();
    await expect(page.locator('input[type="number"]')).not.toBeVisible();
  });

  test('transaction list shows table headers', async ({ page }) => {
    await page.goto('/financeiro/transacoes');

    await expect(page.locator('th:has-text("Data")')).toBeVisible();
    await expect(page.locator('th:has-text("Tipo")')).toBeVisible();
    await expect(page.locator('th:has-text("Valor")')).toBeVisible();
    await expect(page.locator('th:has-text("Status")')).toBeVisible();
  });

  test('export modal opens', async ({ page }) => {
    await page.goto('/financeiro/transacoes');

    await page.locator('button:has-text("Exportar")').click();

    await expect(page.locator('text=Exportar Transações')).toBeVisible();
    await expect(page.locator('select')).toBeVisible();
  });

  test('filter by type works', async ({ page }) => {
    await page.goto('/financeiro/transacoes');

    const tipoSelect = page.locator('select').first();
    await tipoSelect.selectOption('Receita');

    await expect(tipoSelect).toHaveValue('Receita');
  });
});
