import { test, expect } from '@playwright/test';

test.describe('Company Registration Flow', () => {
  test('renders cadastro page with PF/PJ toggle', async ({ page }) => {
    await page.goto('/cadastro');

    await expect(page.locator('h1')).toContainText('Cadastro de Empresa');
    await expect(page.locator('button[role="switch"]')).toHaveCount(2);
    await expect(page.locator('button', { hasText: 'Pessoa Física' })).toHaveAttribute('aria-checked', 'true');
    await expect(page.locator('button', { hasText: 'Pessoa Jurídica' })).toHaveAttribute('aria-checked', 'false');
  });

  test('shows CPF field when PF is selected', async ({ page }) => {
    await page.goto('/cadastro');

    await expect(page.locator('label', { hasText: 'CPF' })).toBeVisible();
    await expect(page.locator('#documento')).toHaveAttribute('placeholder', '000.000.000-00');
    await expect(page.locator('#nome')).toHaveAttribute('aria-describedby', 'nome-help');
  });

  test('switches to CNPJ when PJ is selected', async ({ page }) => {
    await page.goto('/cadastro');

    await page.locator('button', { hasText: 'Pessoa Jurídica' }).click();

    await expect(page.locator('button', { hasText: 'Pessoa Jurídica' })).toHaveAttribute('aria-checked', 'true');
    await expect(page.locator('button', { hasText: 'Pessoa Física' })).toHaveAttribute('aria-checked', 'false');
    await expect(page.locator('label', { hasText: 'CNPJ' })).toBeVisible();
    await expect(page.locator('#documento')).toHaveAttribute('placeholder', '00.000.000/0000-00');
    await expect(page.locator('label', { hasText: 'Razão Social' })).toBeVisible();
  });

  test('validates required fields on submit', async ({ page }) => {
    await page.goto('/cadastro');

    await page.locator('button[type="submit"]').click();

    const nomeInput = page.locator('#nome');
    await expect(nomeInput).toBeFocused();
  });

  test('formats CPF with mask', async ({ page }) => {
    await page.goto('/cadastro');

    const input = page.locator('#documento');
    await input.fill('52998224725');

    await expect(input).toHaveValue('529.982.247-25');
  });

  test('formats CNPJ with mask after switching to PJ', async ({ page }) => {
    await page.goto('/cadastro');

    await page.locator('button', { hasText: 'Pessoa Jurídica' }).click();
    const input = page.locator('#documento');
    await input.fill('12345678000195');

    await expect(input).toHaveValue('12.345.678/0001-95');
  });

  test('formats telefone with mask', async ({ page }) => {
    await page.goto('/cadastro');

    const input = page.locator('#telefone');
    await input.fill('11999998888');

    await expect(input).toHaveValue('(11) 99999-8888');
  });

  test('has link to login page', async ({ page }) => {
    await page.goto('/cadastro');

    const loginLink = page.locator('a', { hasText: 'Entrar' });
    await expect(loginLink).toBeVisible();
    await expect(loginLink).toHaveAttribute('href', '/login');
  });
});
