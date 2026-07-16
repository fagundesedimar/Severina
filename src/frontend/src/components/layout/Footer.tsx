import * as React from "react"
import Link from "next/link"
import { cn } from "@/lib/utils"

interface FooterProps {
  className?: string
}

export function Footer({ className }: FooterProps) {
  const currentYear = new Date().getFullYear()

  return (
    <footer
      className={cn(
        "border-t border-border bg-background py-2 px-4 md:px-6",
        className
      )}
    >
      <div className="max-w-[1440px] mx-auto flex items-center justify-between">
        <div className="flex items-center gap-1.5">
          <span className="material-symbols-outlined text-primary text-sm">
            smart_toy
          </span>
          <span className="text-xs font-medium text-foreground">
            Severina AI
          </span>
        </div>

        <p className="absolute left-1/2 -translate-x-1/2 text-[11px] text-muted-foreground">
          © {currentYear} Severina AI. Todos os direitos reservados.
        </p>

        <div className="flex items-center gap-3">
          <Link
            href="/termos-de-uso"
            className="text-[11px] text-muted-foreground hover:text-foreground transition-colors"
          >
            Termos
          </Link>
          <Link
            href="/privacidade"
            className="text-[11px] text-muted-foreground hover:text-foreground transition-colors"
          >
            Privacidade
          </Link>
          <Link
            href="/suporte"
            className="text-[11px] text-muted-foreground hover:text-foreground transition-colors"
          >
            Suporte
          </Link>
        </div>
      </div>
    </footer>
  )
}
