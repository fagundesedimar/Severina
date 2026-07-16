"use client"

import * as React from "react"
import { cn } from "@/lib/utils"
import { Avatar } from "@/components/ui/avatar"

interface TopAppBarProps {
  title?: string
  subtitle?: string
  avatar?: string
  avatarFallback?: string
  onMenuClick?: () => void
  actions?: React.ReactNode
  className?: string
}

export function TopAppBar({
  title,
  subtitle,
  avatar,
  avatarFallback,
  onMenuClick,
  actions,
  className,
}: TopAppBarProps) {
  return (
    <header
      className={cn(
        "sticky top-0 z-40 flex h-14 items-center gap-4 border-b border-border bg-background/80 backdrop-blur-sm px-4",
        className
      )}
    >
      {onMenuClick && (
        <button
          onClick={onMenuClick}
          className="rounded-lg p-2 text-foreground hover:bg-muted lg:hidden"
          aria-label="Abrir menu"
        >
          <span className="material-symbols-outlined">menu</span>
        </button>
      )}

      <div className="flex h-8 w-8 items-center justify-center rounded-full bg-primary">
        <span className="material-symbols-outlined text-primary-foreground text-lg">
          smart_toy
        </span>
      </div>

      <div className="flex-1 min-w-0">
        {title && (
          <h1 className="text-lg font-bold text-foreground truncate">{title}</h1>
        )}
        {subtitle && (
          <p className="text-xs text-muted-foreground truncate">{subtitle}</p>
        )}
      </div>

      {actions && <div className="flex items-center gap-2">{actions}</div>}

      {avatar !== undefined && (
        <Avatar src={avatar} fallback={avatarFallback || "U"} className="h-8 w-8" />
      )}
    </header>
  )
}
