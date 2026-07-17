import { test, expect } from '@playwright/test';

test.describe('Invite Flow', () => {
  test('shows loading state while validating invite', async ({ page }) => {
    await page.goto('/convite/test-code-123');

    await expect(page.locator('text=Validando convite...')).toBeVisible();
  });

  test('shows error page for invalid invite', async ({ page }) => {
    await page.goto('/convite/invalid-code');

    await expect(page.locator('text=Convite Inválido')).toBeVisible({ timeout: 10000 });
    await expect(page.locator('a', { hasText: 'Ir para o Login' })).toBeVisible();
    await expect(page.locator('a', { hasText: 'Ir para o Login' })).toHaveAttribute('href', '/login');
  });

  test('renders invite acceptance form when valid', async ({ page }) => {
    await page.goto('/convite/valid-test-code');

    await expect(page.locator('h1', { hasText: 'Aceitar Convite' })).toBeVisible({ timeout: 10000 });
    await expect(page.locator('#nome')).toBeVisible();
    await expect(page.locator('#senha')).toBeVisible();
    await expect(page.locator('#confirm-senha')).toBeVisible();
  });

  test('validates password minimum length', async ({ page }) => {
    await page.goto('/convite/valid-test-code');

    await page.locator('#senha').waitFor({ timeout: 10000 });
    await expect(page.locator('#senha')).toHaveAttribute('minlength', '8');
  });

  test('shows password mismatch error', async ({ page }) => {
    await page.goto('/convite/valid-test-code');

    await page.locator('#senha').waitFor({ timeout: 10000 });
    await page.fill('#nome', 'Test User');
    await page.fill('#senha', 'password123');
    await page.fill('#confirm-senha', 'different456');
    await page.locator('button[type="submit"]').click();

    await expect(page.locator('text=As senhas não coincidem')).toBeVisible();
  });

  test('has proper accessibility labels', async ({ page }) => {
    await page.goto('/convite/valid-test-code');

    await page.locator('#senha').waitFor({ timeout: 10000 });
    await expect(page.locator('label', { hasText: 'Seu Nome' })).toBeVisible();
    await expect(page.locator('label', { hasText: 'Crie sua Senha' })).toBeVisible();
    await expect(page.locator('label', { hasText: 'Confirme sua Senha' })).toBeVisible();
    await expect(page.locator('#senha-help')).toContainText('Mínimo de 8 caracteres');
  });

  test('submit button shows loading state', async ({ page }) => {
    await page.goto('/convite/valid-test-code');

    await page.locator('#senha').waitFor({ timeout: 10000 });
    await expect(page.locator('button[type="submit"]')).toContainText('Aceitar Convite');
  });

  test('shows expired invite message for expired code', async ({ page }) => {
    await page.goto('/convite/expired-code');

    await expect(page.locator('text=Convite expirado')).toBeVisible({ timeout: 10000 });
    await expect(page.locator('a', { hasText: 'Ir para o Login' })).toBeVisible();
  });

  test('has back to login link', async ({ page }) => {
    await page.goto('/convite/valid-test-code');

    await page.locator('#senha').waitFor({ timeout: 10000 });
    await expect(page.locator('a', { hasText: 'Voltar para o Login' })).toBeVisible();
    await expect(page.locator('a', { hasText: 'Voltar para o Login' })).toHaveAttribute('href', '/login');
  });

  test('does not submit with empty fields', async ({ page }) => {
    await page.goto('/convite/valid-test-code');

    await page.locator('#senha').waitFor({ timeout: 10000 });
    await page.locator('button[type="submit"]').click();

    await expect(page.locator('#nome')).toHaveAttribute('required');
    await expect(page.locator('#senha')).toHaveAttribute('required');
  });

  test('shows password help text', async ({ page }) => {
    await page.goto('/convite/valid-test-code');

    await page.locator('#senha').waitFor({ timeout: 10000 });
    await expect(page.locator('#senha-help')).toContainText('Mínimo de 8 caracteres');
  });
});
