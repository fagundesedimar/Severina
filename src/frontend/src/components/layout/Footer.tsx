import * as React from "react"
import { cn } from "@/lib/utils"

interface FooterProps {
  className?: string
}

export function Footer({ className }: FooterProps) {
  return (
    <footer
      className={cn(
        "border-t border-border bg-background py-6 px-4 md:px-6",
        className
      )}
    >
      <div className="max-w-[1440px] mx-auto flex flex-col md:flex-row items-center justify-between gap-4">
        <div className="flex items-center gap-2">
          <div className="flex h-6 w-6 items-center justify-center rounded bg-primary">
            <span className="material-symbols-outlined text-primary-foreground text-sm">
              smart_toy
            </span>
          </div>
          <span className="text-sm font-semibold text-foreground">
            Severina AI
          </span>
        </div>

        <p className="text-xs text-muted-foreground text-center">
          Secretária virtual inteligente para pequenas empresas
        </p>

        <div className="flex items-center gap-4">
          <a
            href="#"
            className="text-xs text-muted-foreground hover:text-foreground transition-colors"
          >
            Termos de Uso
          </a>
          <a
            href="#"
            className="text-xs text-muted-foreground hover:text-foreground transition-colors"
          >
            Privacidade
          </a>
          <a
            href="#"
            className="text-xs text-muted-foreground hover:text-foreground transition-colors"
          >
            Suporte
          </a>
        </div>
      </div>
    </footer>
  )
}
