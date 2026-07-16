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
  collapsed?: boolean
  userName?: string
  userRole?: string
  userAvatar?: string
}

export function Menu({
  className,
  collapsed = false,
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
                "flex items-center gap-3 rounded-lg text-sm font-medium transition-colors",
                collapsed ? "justify-center px-2 py-2.5" : "px-3 py-2.5",
                isActive
                  ? "bg-primary/10 text-primary font-semibold"
                  : "text-muted-foreground hover:bg-muted"
              )}
              title={collapsed ? item.label : undefined}
            >
              <span className="material-symbols-outlined text-xl shrink-0">
                {item.icon}
              </span>
              {!collapsed && <span>{item.label}</span>}
            </Link>
          )
        })}

        <div className={cn("pt-6", collapsed ? "px-0" : "px-3")}>
          {!collapsed && (
            <span className="text-[11px] font-semibold uppercase tracking-widest text-muted-foreground">
              Sistema
            </span>
          )}
          {collapsed && <div className="border-t border-border mx-2" />}
        </div>

        {systemItems.map((item) => {
          const isActive = pathname.startsWith(item.href)

          return (
            <Link
              key={item.href}
              href={item.href}
              className={cn(
                "flex items-center gap-3 rounded-lg text-sm font-medium transition-colors",
                collapsed ? "justify-center px-2 py-2.5" : "px-3 py-2.5",
                isActive
                  ? "bg-primary/10 text-primary font-semibold"
                  : "text-muted-foreground hover:bg-muted"
              )}
              title={collapsed ? item.label : undefined}
            >
              <span className="material-symbols-outlined text-xl shrink-0">
                {item.icon}
              </span>
              {!collapsed && <span>{item.label}</span>}
            </Link>
          )
        })}
      </div>

      <div className={cn("p-3 mt-auto", collapsed && "p-2")}>
        {collapsed ? (
          <div className="flex justify-center">
            <Avatar
              src={userAvatar}
              fallback={userName.charAt(0)}
              className="h-10 w-10"
            />
          </div>
        ) : (
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
        )}
      </div>
    </nav>
  )
}
