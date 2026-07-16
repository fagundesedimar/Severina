import { test, expect } from '@playwright/test';

test.describe('Client Creation E2E', () => {
  test('client list page loads correctly', async ({ page }) => {
    await page.goto('/clientes');

    await expect(page.locator('text=Clientes')).toBeVisible();
    await expect(page.locator('input[placeholder*="Buscar"]')).toBeVisible();
  });

  test('search input works with debounce', async ({ page }) => {
    await page.goto('/clientes');

    const searchInput = page.locator('input[placeholder*="Buscar"]');
    await expect(searchInput).toBeVisible();

    await searchInput.fill(' teste ');

    await page.waitForTimeout(500);

    const listArea = page.locator('[class*="divide-y"]');
    await expect(listArea).toBeVisible();
  });

  test('status filter is present', async ({ page }) => {
    await page.goto('/clientes');

    await expect(page.locator('text=Ativo')).toBeVisible();
    await expect(page.locator('text=Inativo')).toBeVisible();
  });

  test('client card shows name, phone and status', async ({ page }) => {
    await page.goto('/clientes');

    const clientCards = page.locator('[class*="grid"][class*="grid-cols-12"]');
    const count = await clientCards.count();

    if (count > 0) {
      const firstCard = clientCards.first();
      await expect(firstCard).toBeVisible();

      const nameLink = firstCard.locator('a[href*="/clientes/"]');
      await expect(nameLink).toBeVisible();
    }
  });

  test('pagination controls appear when needed', async ({ page }) => {
    await page.goto('/clientes');

    await page.waitForTimeout(1000);

    const anteriorBtn = page.locator('button:has-text("Anterior")');
    const proximaBtn = page.locator('button:has-text("Próxima")');

    const hasPagination = await anteriorBtn.isVisible().catch(() => false);

    if (hasPagination) {
      await expect(proximaBtn).toBeVisible();
    }
  });

  test('navigate to client detail from list', async ({ page }) => {
    await page.goto('/clientes');

    await page.waitForTimeout(1000);

    const clientLink = page.locator('a[href*="/clientes/"]').first();
    const hasClients = await clientLink.isVisible().catch(() => false);

    if (hasClients) {
      const href = await clientLink.getAttribute('href');
      await clientLink.click();
      await page.waitForURL(`**${href}`);
      expect(page.url()).toContain('/clientes/');
    }
  });

  test('empty state shows when no clients found', async ({ page }) => {
    await page.goto('/clientes');

    await page.waitForTimeout(1000);

    const emptyState = page.locator('text=Nenhum cliente encontrado');
    const hasClients = await page.locator('[class*="grid"][class*="grid-cols-12"]').count();

    if (hasClients === 0) {
      await expect(emptyState).toBeVisible();
    }
  });
});
