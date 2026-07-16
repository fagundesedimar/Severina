"use client"

import * as React from "react"
import Link from "next/link"
import { usePathname } from "next/navigation"
import { cn } from "@/lib/utils"
import { Avatar } from "@/components/ui/avatar"

interface MenuItem {
  label: string
  href: string
  icon: string
}

const menuItems: MenuItem[] = [
  { label: "Início", href: "/dashboard", icon: "home" },
  { label: "Clientes", href: "/clientes", icon: "group" },
  { label: "Agenda", href: "/agenda", icon: "calendar_today" },
  { label: "Financeiro", href: "/financeiro", icon: "payments" },
]

const systemItems: MenuItem[] = [
  { label: "Configurações", href: "/configuracoes/usuarios", icon: "settings" },
]

interface MenuProps {
  className?: string
  userName?: string
  userRole?: string
  userAvatar?: string
}

export function Menu({
  className,
  userName = "Maria Severina",
  userRole = "Administradora",
  userAvatar,
}: MenuProps) {
  const pathname = usePathname()

  return (
    <nav className={cn("flex flex-col h-full", className)}>
      <div className="flex-1 p-3 space-y-1">
        {menuItems.map((item) => {
          const isActive =
            pathname === item.href ||
            (item.href !== "/dashboard" && pathname.startsWith(item.href))

          return (
            <Link
              key={item.href}
              href={item.href}
              className={cn(
                "flex items-center gap-3 px-3 py-2 rounded-lg text-sm font-medium transition-colors",
                isActive
                  ? "bg-primary/10 text-primary font-semibold"
                  : "text-muted-foreground hover:bg-muted"
              )}
            >
              <span className="material-symbols-outlined text-xl">
                {item.icon}
              </span>
              <span>{item.label}</span>
            </Link>
          )
        })}

        <div className="pt-6 opacity-50 px-3">
          <span className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
            Sistema
          </span>
        </div>

        {systemItems.map((item) => {
          const isActive = pathname.startsWith(item.href)

          return (
            <Link
              key={item.href}
              href={item.href}
              className={cn(
                "flex items-center gap-3 px-3 py-2 rounded-lg text-sm font-medium transition-colors",
                isActive
                  ? "bg-primary/10 text-primary font-semibold"
                  : "text-muted-foreground hover:bg-muted"
              )}
            >
              <span className="material-symbols-outlined text-xl">
                {item.icon}
              </span>
              <span>{item.label}</span>
            </Link>
          )
        })}
      </div>

      <div className="p-3 mt-auto">
        <div className="flex items-center gap-3 p-3 rounded-xl bg-muted border border-border">
          <Avatar
            src={userAvatar}
            fallback={userName.charAt(0)}
            className="h-10 w-10"
          />
          <div className="flex flex-col min-w-0">
            <span className="text-sm font-semibold text-foreground truncate">
              {userName}
            </span>
            <span className="text-xs text-muted-foreground truncate">
              {userRole}
            </span>
          </div>
        </div>
      </div>
    </nav>
  )
}
