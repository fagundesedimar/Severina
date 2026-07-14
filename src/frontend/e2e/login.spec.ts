import { test, expect } from '@playwright/test';

test.describe('Login Flow', () => {
  test('login page renders correctly', async ({ page }) => {
    await page.goto('/login');
    
    await expect(page.locator('h1')).toContainText('Entrar na Severina');
    await expect(page.locator('input[type="email"]')).toBeVisible();
    await expect(page.locator('input[type="password"]')).toBeVisible();
    await expect(page.locator('button[type="submit"]')).toContainText('Entrar');
  });

  test('theme toggle works', async ({ page }) => {
    await page.goto('/login');
    
    const themeButton = page.locator('button[aria-label="Alternar tema"]');
    await expect(themeButton).toBeVisible();
    
    const initialTheme = await page.evaluate(() => 
      document.documentElement.getAttribute('data-theme')
    );
    
    await themeButton.click();
    
    const newTheme = await page.evaluate(() => 
      document.documentElement.getAttribute('data-theme')
    );
    
    expect(newTheme).not.toBe(initialTheme);
  });

  test('password visibility toggle works', async ({ page }) => {
    await page.goto('/login');
    
    const passwordInput = page.locator('input[type="password"]');
    await expect(passwordInput).toBeVisible();
    
    const toggleButton = page.locator('button[aria-label="Mostrar senha"]');
    await toggleButton.click();
    
    const textInput = page.locator('input[type="text"]');
    await expect(textInput).toBeVisible();
  });

  test('form validation - empty fields', async ({ page }) => {
    await page.goto('/login');
    
    const submitButton = page.locator('button[type="submit"]');
    await submitButton.click();
    
    const emailInput = page.locator('input[type="email"]');
    await expect(emailInput).toBeFocused();
  });
});
