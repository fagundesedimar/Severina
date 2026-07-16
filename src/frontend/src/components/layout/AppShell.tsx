"use client"

import * as React from "react"
import { useRouter } from "next/navigation"
import { useAuthStore } from "@/stores/useAuthStore"
import { Menu } from "./Menu"
import { TopAppBar } from "./TopAppBar"
import { BottomNavBar } from "./BottomNavBar"
import { Footer } from "./Footer"
import { ThemeToggle } from "./ThemeToggle"

interface AppShellProps {
  children: React.ReactNode
  title?: string
  actions?: React.ReactNode
}

export function AppShell({ children, title, actions }: AppShellProps) {
  const [sidebarOpen, setSidebarOpen] = React.useState(false)
  const { isAuthenticated, user } = useAuthStore()
  const router = useRouter()

  React.useEffect(() => {
    if (!isAuthenticated) {
      router.push("/login")
    }
  }, [isAuthenticated, router])

  if (!isAuthenticated) return null

  return (
    <div className="min-h-screen bg-background">
      {/* Mobile overlay */}
      {sidebarOpen && (
        <div
          className="fixed inset-0 z-40 bg-black/50 lg:hidden"
          onClick={() => setSidebarOpen(false)}
        />
      )}

      {/* Desktop Sidebar */}
      <aside className="hidden lg:flex fixed left-0 top-0 h-full w-64 bg-background border-r border-border flex-col z-50">
        <div className="p-4 border-b border-border flex items-center justify-between">
          <div className="flex items-center gap-2">
            <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-primary">
              <span className="material-symbols-outlined text-primary-foreground text-lg">
                smart_toy
              </span>
            </div>
            <span className="text-base font-bold text-foreground">
              Severina AI
            </span>
          </div>
          <ThemeToggle />
        </div>

        <Menu
          userName={user?.nome || "Usuário"}
          userRole={user?.papel || ""}
        />
      </aside>

      {/* Mobile Sidebar (Drawer) */}
      <aside
        className={`fixed left-0 top-0 h-full w-64 bg-background border-r border-border flex flex-col z-50 transition-transform duration-300 lg:hidden ${
          sidebarOpen ? "translate-x-0" : "-translate-x-full"
        }`}
      >
        <div className="p-4 border-b border-border flex items-center justify-between">
          <div className="flex items-center gap-2">
            <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-primary">
              <span className="material-symbols-outlined text-primary-foreground text-lg">
                smart_toy
              </span>
            </div>
            <span className="text-base font-bold text-foreground">
              Severina AI
            </span>
          </div>
          <div className="flex items-center gap-1">
            <ThemeToggle />
            <button
              onClick={() => setSidebarOpen(false)}
              className="p-2 rounded-lg hover:bg-muted"
            >
              <span className="material-symbols-outlined">close</span>
            </button>
          </div>
        </div>

        <Menu
          userName={user?.nome || "Usuário"}
          userRole={user?.papel || ""}
        />
      </aside>

      {/* Mobile TopAppBar + BottomNav */}
      <div className="lg:hidden">
        <TopAppBar
          title={title}
          onMenuClick={() => setSidebarOpen(true)}
          actions={
            <>
              <ThemeToggle />
              {actions}
            </>
          }
          avatarFallback={user?.nome?.charAt(0)?.toUpperCase() || "U"}
        />
        <BottomNavBar />
      </div>

      {/* Main Content */}
      <div className="lg:pl-64">
        {/* Desktop Header */}
        <header className="sticky top-0 z-30 hidden lg:flex h-16 items-center gap-4 border-b border-border bg-background/80 backdrop-blur-sm px-6">
          <div className="flex-1">
            {title && (
              <h1 className="text-lg font-bold text-foreground">{title}</h1>
            )}
          </div>
          <div className="flex items-center gap-2">
            <ThemeToggle />
            {actions}
          </div>
        </header>

        {/* Page Content */}
        <main className="p-4 pb-20 lg:p-6 lg:pb-6 min-h-[calc(100vh-4rem)]">
          {children}
        </main>

        {/* Footer */}
        <Footer />
      </div>
    </div>
  )
}
