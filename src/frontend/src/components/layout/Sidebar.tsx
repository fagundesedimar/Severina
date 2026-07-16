"use client"

import * as React from "react"
import Link from "next/link"
import { usePathname, useRouter } from "next/navigation"
import { cn } from "@/lib/utils"
import { useAuthStore } from "@/stores/useAuthStore"

interface SidebarProps {
  open: boolean
  onToggle: () => void
}

interface NavItem {
  label: string
  href: string
  icon: string
  badge?: number
}

const navItems: NavItem[] = [
  { label: "Dashboard", href: "/dashboard", icon: "dashboard" },
  { label: "Atendimento", href: "/atendimento/novo", icon: "support_agent" },
  { label: "Clientes", href: "/clientes", icon: "people" },
  { label: "Agenda", href: "/agenda", icon: "calendar_month" },
  { label: "Financeiro", href: "/financeiro", icon: "payments" },
  { label: "Cobranças", href: "/financeiro/cobrancas", icon: "receipt_long" },
]

const bottomNavItems: NavItem[] = [
  { label: "Configurações", href: "/configuracoes/usuarios", icon: "settings" },
]

export function Sidebar({ open, onToggle }: SidebarProps) {
  const pathname = usePathname()
  const router = useRouter()
  const { user, logout } = useAuthStore()

  const handleLogout = () => {
    logout()
    router.push("/login")
  }

  return (
    <>
      {/* Mobile overlay */}
      {open && (
        <div
          className="fixed inset-0 z-40 bg-black/50 lg:hidden"
          onClick={onToggle}
        />
      )}

      {/* Sidebar */}
      <aside
        className={cn(
          "fixed left-0 top-0 z-50 flex h-full w-64 flex-col bg-sidebar text-sidebar-foreground transition-transform duration-300 lg:translate-x-0",
          open ? "translate-x-0" : "-translate-x-full"
        )}
      >
        {/* Logo */}
        <div className="flex h-16 items-center gap-3 border-b border-sidebar-border px-6">
          <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-white/15">
            <span className="material-symbols-outlined text-white text-xl">
              smart_toy
            </span>
          </div>
          <div>
            <h1 className="text-base font-bold text-white leading-tight">
              Severina AI
            </h1>
            <p className="text-[11px] text-sidebar-foreground/60">
              Secretária Virtual
            </p>
          </div>
        </div>

        {/* Navigation */}
        <nav className="flex-1 overflow-y-auto px-3 py-4">
          <div className="space-y-1">
            {navItems.map((item) => {
              const isActive =
                pathname === item.href ||
                (item.href !== "/dashboard" && pathname.startsWith(item.href.split("?")[0]))
              return (
                <Link
                  key={item.href}
                  href={item.href}
                  className={cn(
                    "flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition-colors",
                    isActive
                      ? "bg-white/15 text-white"
                      : "text-sidebar-foreground/70 hover:bg-white/8 hover:text-white"
                  )}
                >
                  <span className="material-symbols-outlined text-xl">
                    {item.icon}
                  </span>
                  {item.label}
                </Link>
              )
            })}
          </div>

          <div className="mt-6 border-t border-sidebar-border pt-4">
            <p className="mb-2 px-3 text-[11px] font-medium uppercase tracking-wider text-sidebar-foreground/40">
              Sistema
            </p>
            {bottomNavItems.map((item) => {
              const isActive = pathname.startsWith(item.href)
              return (
                <Link
                  key={item.href}
                  href={item.href}
                  className={cn(
                    "flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition-colors",
                    isActive
                      ? "bg-white/15 text-white"
                      : "text-sidebar-foreground/70 hover:bg-white/8 hover:text-white"
                  )}
                >
                  <span className="material-symbols-outlined text-xl">
                    {item.icon}
                  </span>
                  {item.label}
                </Link>
              )
            })}
          </div>
        </nav>

        {/* User section */}
        <div className="border-t border-sidebar-border p-4">
          <div className="flex items-center gap-3">
            <div className="flex h-9 w-9 items-center justify-center rounded-full bg-white/15 text-sm font-medium text-white">
              {user?.nome?.charAt(0)?.toUpperCase() || "U"}
            </div>
            <div className="flex-1 min-w-0">
              <p className="text-sm font-medium text-white truncate">
                {user?.nome || "Usuário"}
              </p>
              <p className="text-[11px] text-sidebar-foreground/60 truncate">
                {user?.email || ""}
              </p>
            </div>
            <button
              onClick={handleLogout}
              className="rounded-lg p-1.5 text-sidebar-foreground/60 hover:bg-white/8 hover:text-white transition-colors"
              title="Sair"
            >
              <span className="material-symbols-outlined text-xl">
                logout
              </span>
            </button>
          </div>
        </div>
      </aside>
    </>
  )
}
