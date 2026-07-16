import Link from "next/link";

export default function SuportePage() {
  return (
    <div className="min-h-screen bg-background">
      <div className="max-w-3xl mx-auto px-4 py-12 lg:py-20">
        <Link
          href="/"
          className="inline-flex items-center gap-2 text-sm text-muted-foreground hover:text-foreground transition-colors mb-8"
        >
          <span className="material-symbols-outlined text-lg">arrow_back</span>
          Voltar
        </Link>

        <h1 className="text-3xl font-bold text-foreground mb-2">
          Central de Suporte
        </h1>
        <p className="text-sm text-muted-foreground mb-8">
          Estamos aqui para ajudar. Encontre respostas ou entre em contato.
        </p>

        <div className="space-y-8">
          <section>
            <h2 className="text-xl font-bold text-foreground mb-4">Canais de Atendimento</h2>
            <div className="grid gap-4 sm:grid-cols-2">
              <div className="border border-border rounded-lg p-5">
                <div className="flex items-center gap-3 mb-2">
                  <span className="material-symbols-outlined text-primary">support_agent</span>
                  <h3 className="font-semibold text-foreground">Chat ao Vivo</h3>
                </div>
                <p className="text-sm text-muted-foreground mb-3">
                  Atendimento em horário comercial (segunda a sexta, 9h às 18h).
                </p>
                <span className="inline-flex items-center gap-1.5 text-sm font-medium text-primary">
                  <span className="w-2 h-2 rounded-full bg-green-500"></span>
                  Online agora
                </span>
              </div>

              <div className="border border-border rounded-lg p-5">
                <div className="flex items-center gap-3 mb-2">
                  <span className="material-symbols-outlined text-primary">mail</span>
                  <h3 className="font-semibold text-foreground">E-mail</h3>
                </div>
                <p className="text-sm text-muted-foreground mb-3">
                  Resposta em até 24h em dias úteis.
                </p>
                <a
                  href="mailto:suporte@severina.ai"
                  className="text-sm font-medium text-primary hover:underline"
                >
                  suporte@severina.ai
                </a>
              </div>

              <div className="border border-border rounded-lg p-5">
                <div className="flex items-center gap-3 mb-2">
                  <span className="material-symbols-outlined text-primary">chat</span>
                  <h3 className="font-semibold text-foreground">WhatsApp</h3>
                </div>
                <p className="text-sm text-muted-foreground mb-3">
                  Suporte via mensagens durante horário comercial.
                </p>
                <a
                  href="https://wa.me/5511999990000"
                  target="_blank"
                  rel="noopener noreferrer"
                  className="text-sm font-medium text-primary hover:underline"
                >
                  (11) 99999-0000
                </a>
              </div>

              <div className="border border-border rounded-lg p-5">
                <div className="flex items-center gap-3 mb-2">
                  <span className="material-symbols-outlined text-primary">description</span>
                  <h3 className="font-semibold text-foreground">Documentação</h3>
                </div>
                <p className="text-sm text-muted-foreground mb-3">
                  Guias, tutoriais e referências da plataforma.
                </p>
                <span className="text-sm font-medium text-muted-foreground">
                  Em breve
                </span>
              </div>
            </div>
          </section>

          <section>
            <h2 className="text-xl font-bold text-foreground mb-4">Perguntas Frequentes</h2>
            <div className="space-y-4">
              <details className="group border border-border rounded-lg">
                <summary className="flex items-center justify-between gap-4 cursor-pointer px-5 py-4 font-medium text-foreground">
                  Como configuro a integração com WhatsApp?
                  <span className="material-symbols-outlined text-muted-foreground transition-transform group-open:rotate-180">
                    expand_more
                  </span>
                </summary>
                <div className="px-5 pb-4 text-sm text-muted-foreground">
                  Acesse <strong>Configurações</strong> {">"} <strong>Integrações</strong> {">"} <strong>WhatsApp</strong> e siga o passo a passo de vinculação. Você precisará de uma conta WhatsApp Business API ativa.
                </div>
              </details>

              <details className="group border border-border rounded-lg">
                <summary className="flex items-center justify-between gap-4 cursor-pointer px-5 py-4 font-medium text-foreground">
                  Meus dados estão seguros?
                  <span className="material-symbols-outlined text-muted-foreground transition-transform group-open:rotate-180">
                    expand_more
                  </span>
                </summary>
                <div className="px-5 pb-4 text-sm text-muted-foreground">
                  Sim. Utilizamos criptografia TLS 1.3 para dados em trânsito e AES-256 para dados em repouso. Cada empresa opera em um ambiente isolado (multi-tenant). Consulte nossa <Link href="/privacidade" className="text-primary hover:underline">Política de Privacidade</Link> para mais detalhes.
                </div>
              </details>

              <details className="group border border-border rounded-lg">
                <summary className="flex items-center justify-between gap-4 cursor-pointer px-5 py-4 font-medium text-foreground">
                  Como funciona o agendamento automático?
                  <span className="material-symbols-outlined text-muted-foreground transition-transform group-open:rotate-180">
                    expand_more
                  </span>
                </summary>
                <div className="px-5 pb-4 text-sm text-muted-foreground">
                  O cliente envia uma mensagem indicando o desejo de agendar. A Severina AI verifica disponibilidade na sua agenda, propõe horários, confirma o compromisso e envia lembretes automáticos — tudo de forma conversacional.
                </div>
              </details>

              <details className="group border border-border rounded-lg">
                <summary className="flex items-center justify-between gap-4 cursor-pointer px-5 py-4 font-medium text-foreground">
                  Posso cancelar minha assinatura a qualquer momento?
                  <span className="material-symbols-outlined text-muted-foreground transition-transform group-open:rotate-180">
                    expand_more
                  </span>
                </summary>
                <div className="px-5 pb-4 text-sm text-muted-foreground">
                  Sim. Acesse <strong>Configurações</strong> {">"} <strong>Plano</strong> e clique em <strong>Cancelar assinatura</strong>. Você continuará com acesso até o final do período já pago.
                </div>
              </details>

              <details className="group border border-border rounded-lg">
                <summary className="flex items-center justify-between gap-4 cursor-pointer px-5 py-4 font-medium text-foreground">
                  Como exporto meus dados?
                  <span className="material-symbols-outlined text-muted-foreground transition-transform group-open:rotate-180">
                    expand_more
                  </span>
                </summary>
                <div className="px-5 pb-4 text-sm text-muted-foreground">
                  Acesse <strong>Configurações</strong> {">"} <strong>Dados</strong> {">"} <strong>Exportar dados</strong>. Você pode exportar seus dados em formatos CSV ou JSON. Conforme a LGPD, você também pode solicitar a exportação completa via e-mail.
                </div>
              </details>

              <details className="group border border-border rounded-lg">
                <summary className="flex items-center justify-between gap-4 cursor-pointer px-5 py-4 font-medium text-foreground">
                  A IA erra nas respostas? Como corrijo?
                  <span className="material-symbols-outlined text-muted-foreground transition-transform group-open:rotate-180">
                    expand_more
                  </span>
                </summary>
                <div className="px-5 pb-4 text-sm text-muted-foreground">
                  A IA é treinada com base nas informações da sua empresa e melhora continuamente. Você pode ajustar respostas em <strong>Configurações</strong> {">"} <strong>IA</strong> {">"} <strong>Personalização</strong>. Respostas incorretas podem ser corrigidas para treinar o modelo.
                </div>
              </details>
            </div>
          </section>

          <section>
            <h2 className="text-xl font-bold text-foreground mb-4">Reportar um Problema</h2>
            <div className="border border-border rounded-lg p-5">
              <p className="text-sm text-muted-foreground mb-4">
                Encontrou um bug ou tem uma sugestão de melhoria? Nos ajude a melhorar.
              </p>
              <a
                href="mailto:suporte@severina.ai?subject=Reporte de Problema"
                className="inline-flex items-center gap-2 px-4 py-2 bg-primary text-primary-foreground rounded-lg text-sm font-medium hover:opacity-90 transition-opacity"
              >
                <span className="material-symbols-outlined text-lg">bug_report</span>
                Reportar problema
              </a>
            </div>
          </section>
        </div>
      </div>
    </div>
  );
}
