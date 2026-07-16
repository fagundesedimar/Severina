"use client"

import * as React from "react"
import { cn } from "@/lib/utils"
import { useThemeStore } from "@/stores/useThemeStore"

interface ThemeToggleProps {
  className?: string
  variant?: "icon" | "dropdown"
}

export function ThemeToggle({ className, variant = "icon" }: ThemeToggleProps) {
  const { theme, setTheme, resolvedTheme } = useThemeStore()
  const [isOpen, setIsOpen] = React.useState(false)

  const getIcon = () => {
    if (theme === "system") {
      return resolvedTheme === "dark" ? "dark_mode" : "light_mode"
    }
    return theme === "dark" ? "dark_mode" : "light_mode"
  }

  const cycleTheme = () => {
    const themes: Array<"light" | "dark" | "system"> = ["light", "dark", "system"]
    const currentIndex = themes.indexOf(theme)
    const nextIndex = (currentIndex + 1) % themes.length
    setTheme(themes[nextIndex])
  }

  if (variant === "icon") {
    return (
      <button
        onClick={cycleTheme}
        className={cn(
          "p-2 rounded-full hover:bg-muted transition-colors active:scale-95",
          className
        )}
        title={`Tema atual: ${theme === "system" ? "Sistema" : theme === "dark" ? "Escuro" : "Claro"}`}
      >
        <span className="material-symbols-outlined text-xl">
          {getIcon()}
        </span>
      </button>
    )
  }

  return (
    <div className={cn("relative", className)}>
      <button
        onClick={() => setIsOpen(!isOpen)}
        className="flex items-center gap-2 px-3 py-2 rounded-lg hover:bg-muted transition-colors"
      >
        <span className="material-symbols-outlined text-xl">
          {getIcon()}
        </span>
        <span className="text-sm font-medium capitalize">
          {theme === "system" ? "Sistema" : theme === "dark" ? "Escuro" : "Claro"}
        </span>
        <span className="material-symbols-outlined text-sm">
          {isOpen ? "expand_less" : "expand_more"}
        </span>
      </button>

      {isOpen && (
        <>
          <div
            className="fixed inset-0 z-40"
            onClick={() => setIsOpen(false)}
          />
          <div className="absolute right-0 mt-2 w-40 bg-card border border-border rounded-lg shadow-lg z-50 overflow-hidden">
            {(["light", "dark", "system"] as const).map((option) => (
              <button
                key={option}
                onClick={() => {
                  setTheme(option)
                  setIsOpen(false)
                }}
                className={cn(
                  "w-full flex items-center gap-3 px-4 py-2.5 text-sm transition-colors",
                  theme === option
                    ? "bg-primary/10 text-primary font-semibold"
                    : "text-foreground hover:bg-muted"
                )}
              >
                <span className="material-symbols-outlined text-lg">
                  {option === "light" ? "light_mode" : option === "dark" ? "dark_mode" : "computer"}
                </span>
                <span className="capitalize">
                  {option === "light" ? "Claro" : option === "dark" ? "Escuro" : "Sistema"}
                </span>
                {theme === option && (
                  <span className="material-symbols-outlined text-lg ml-auto">
                    check
                  </span>
                )}
              </button>
            ))}
          </div>
        </>
      )}
    </div>
  )
}
