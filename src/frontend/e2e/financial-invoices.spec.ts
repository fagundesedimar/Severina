import { test, expect } from '@playwright/test';

test.describe('Invoices Page', () => {
  test('invoices page renders', async ({ page }) => {
    await page.goto('/financeiro/cobrancas');

    await expect(page.locator('h1')).toContainText('Cobranças');
    await expect(page.locator('button:has-text("Nova Fatura")')).toBeVisible();
    await expect(page.locator('button:has-text("Exportar")')).toBeVisible();
  });

  test('create invoice form shows on button click', async ({ page }) => {
    await page.goto('/financeiro/cobrancas');

    await page.locator('button:has-text("Nova Fatura")').click();

    await expect(page.locator('input[type="number"]')).toBeVisible();
    await expect(page.locator('input[type="date"]')).toBeVisible();
  });

  test('invoice list shows table headers', async ({ page }) => {
    await page.goto('/financeiro/cobrancas');

    await expect(page.locator('th:has-text("Nº")')).toBeVisible();
    await expect(page.locator('th:has-text("Vencimento")')).toBeVisible();
    await expect(page.locator('th:has-text("Valor")')).toBeVisible();
    await expect(page.locator('th:has-text("Status")')).toBeVisible();
    await expect(page.locator('th:has-text("Ações")')).toBeVisible();
  });

  test('status filter shows all options', async ({ page }) => {
    await page.goto('/financeiro/cobrancas');

    const statusSelect = page.locator('select').first();
    await statusSelect.click();

    await expect(page.locator('option:has-text("Pendente")')).toBeVisible();
    await expect(page.locator('option:has-text("Paga")')).toBeVisible();
    await expect(page.locator('option:has-text("Atrasada")')).toBeVisible();
    await expect(page.locator('option:has-text("Cancelada")')).toBeVisible();
  });

  test('export modal opens', async ({ page }) => {
    await page.goto('/financeiro/cobrancas');

    await page.locator('button:has-text("Exportar")').click();

    await expect(page.locator('text=Exportar Faturas')).toBeVisible();
  });
});
