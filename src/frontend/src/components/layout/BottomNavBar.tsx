"use client"

import * as React from "react"
import Link from "next/link"
import { usePathname } from "next/navigation"
import { cn } from "@/lib/utils"

interface NavItem {
  label: string
  href: string
  icon: string
}

const navItems: NavItem[] = [
  { label: "Início", href: "/dashboard", icon: "home" },
  { label: "Clientes", href: "/clientes", icon: "people" },
  { label: "Agenda", href: "/agenda", icon: "calendar_month" },
  { label: "Financeiro", href: "/financeiro", icon: "payments" },
]

interface BottomNavBarProps {
  className?: string
}

export function BottomNavBar({ className }: BottomNavBarProps) {
  const pathname = usePathname()

  return (
    <nav
      className={cn(
        "fixed bottom-0 left-0 right-0 z-40 flex h-16 items-center justify-around border-t border-border bg-background/95 backdrop-blur-sm lg:hidden",
        className
      )}
    >
      {navItems.map((item) => {
        const isActive =
          pathname === item.href ||
          (item.href !== "/dashboard" && pathname.startsWith(item.href))

        return (
          <Link
            key={item.href}
            href={item.href}
            className={cn(
              "flex flex-col items-center gap-0.5 px-3 py-1.5 text-xs transition-colors",
              isActive
                ? "text-primary font-semibold"
                : "text-muted-foreground font-medium"
            )}
          >
            <span
              className={cn(
                "material-symbols-outlined text-2xl",
                isActive && "text-primary"
              )}
              style={isActive ? { fontVariationSettings: "'FILL' 1" } : undefined}
            >
              {item.icon}
            </span>
            <span>{item.label}</span>
          </Link>
        )
      })}
    </nav>
  )
}
