"use client"

import * as React from "react"
import { useRouter } from "next/navigation"
import { useAuthStore } from "@/stores/useAuthStore"
import { TopAppBar } from "./TopAppBar"
import { BottomNavBar } from "./BottomNavBar"

interface MobileLayoutProps {
  children: React.ReactNode
  title?: string
  subtitle?: string
  actions?: React.ReactNode
}

export function MobileLayout({ children, title, subtitle, actions }: MobileLayoutProps) {
  const { isAuthenticated, user } = useAuthStore()
  const router = useRouter()

  React.useEffect(() => {
    if (!isAuthenticated) {
      router.push("/login")
    }
  }, [isAuthenticated, router])

  if (!isAuthenticated) return null

  return (
    <div className="min-h-screen bg-background pb-16">
      <TopAppBar
        title={title}
        subtitle={subtitle}
        actions={actions}
        avatarFallback={user?.nome?.charAt(0)?.toUpperCase() || "U"}
      />
      <main className="p-4">{children}</main>
      <BottomNavBar />
    </div>
  )
}
