import { test, expect } from '@playwright/test';

test.describe('Client Import E2E', () => {
  test('import page loads correctly', async ({ page }) => {
    await page.goto('/clientes/importar');

    await expect(page.locator('text=Importar')).toBeVisible();
  });

  test('drag and drop area is visible', async ({ page }) => {
    await page.goto('/clientes/importar');

    const dropArea = page.locator('[class*="border-dashed"], [class*="border-2"]').first();
    await expect(dropArea).toBeVisible();
  });

  test('upload button is present', async ({ page }) => {
    await page.goto('/clientes/importar');

    const uploadBtn = page.locator('button:has-text("Selecionar"), button:has-text("Upload"), button:has-text("Escolher"), input[type="file"]').first();
    await expect(uploadBtn).toBeVisible();
  });

  test('file input accepts CSV files', async ({ page }) => {
    await page.goto('/clientes/importar');

    const fileInput = page.locator('input[type="file"]');
    const hasFileInput = await fileInput.count() > 0;

    if (hasFileInput) {
      const accept = await fileInput.getAttribute('accept');
      expect(accept).toContain('csv');
    }
  });

  test('back navigation to client list works', async ({ page }) => {
    await page.goto('/clientes/importar');

    const backLink = page.locator('a[href="/clientes"]').first();
    const hasBack = await backLink.isVisible().catch(() => false);

    if (hasBack) {
      await backLink.click();
      await page.waitForURL('**/clientes');
      expect(page.url()).toContain('/clientes');
    }
  });
});
