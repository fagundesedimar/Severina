import Link from "next/link";

export default function TermosDeUsoPage() {
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
          Termos de Uso
        </h1>
        <p className="text-sm text-muted-foreground mb-8">
          Última atualização: Julho de 2026
        </p>

        <div className="prose prose-neutral dark:prose-invert max-w-none space-y-8 text-foreground">
          <section>
            <h2 className="text-xl font-bold text-foreground mb-3">1. Aceitação dos Termos</h2>
            <p className="text-muted-foreground leading-relaxed">
              Ao acessar ou utilizar a plataforma Severina AI (&quot;Plataforma&quot;), você concorda com estes Termos de Uso. Caso não concorde, não utilize a Plataforma. A Severina AI é uma secretária virtual baseada em inteligência artificial destinada a automatizar atendimento omnichannel, agendamentos, cobranças, follow-up e geração de insights para pequenas empresas e profissionais autônomos.
            </p>
          </section>

          <section>
            <h2 className="text-xl font-bold text-foreground mb-3">2. Descrição do Serviço</h2>
            <p className="text-muted-foreground leading-relaxed">
              A Severina AI oferece as seguintes funcionalidades principais:
            </p>
            <ul className="list-disc list-inside text-muted-foreground space-y-2 mt-3">
              <li>Atendimento omnichannel integrado (WhatsApp, webchat e outros canais)</li>
              <li>Agendamento inteligente com validação automática de conflitos</li>
              <li>Cobrança automática com follow-up e lembretes</li>
              <li>CRM de clientes com histórico de interações</li>
              <li>Dashboard de insights com métricas de negócio</li>
              <li>Gestão de usuários e permissões (RBAC)</li>
            </ul>
          </section>

          <section>
            <h2 className="text-xl font-bold text-foreground mb-3">3. Elegibilidade</h2>
            <p className="text-muted-foreground leading-relaxed">
              Para utilizar a Plataforma, você deve: (a) ter pelo menos 18 anos de idade; (b) possuir capacidade legal para celebrar contratos; (c) ser proprietário, representante legal ou autorizado de uma empresa ou atividade comercial; (d) fornecer informações de registro precisas e completas.
            </p>
          </section>

          <section>
            <h2 className="text-xl font-bold text-foreground mb-3">4. Conta e Segurança</h2>
            <p className="text-muted-foreground leading-relaxed">
              Você é responsável por manter a confidencialidade das credenciais de sua conta e por todas as atividades que ocorram sob sua conta. A Plataforma utiliza autenticação JWT com suporte a autenticação de múltiplos fatores (MFA). Você deve notificar imediatamente a Severina AI sobre qualquer uso não autorizado de sua conta.
            </p>
          </section>

          <section>
            <h2 className="text-xl font-bold text-foreground mb-3">5. Uso Aceitável</h2>
            <p className="text-muted-foreground leading-relaxed">
              Você concorda em utilizar a Plataforma em conformidade com a legislação brasileira vigente, incluindo a Lei Geral de Proteção de Dados (LGPD). É proibido:
            </p>
            <ul className="list-disc list-inside text-muted-foreground space-y-2 mt-3">
              <li>Utilizar a Plataforma para fins ilícitos ou não autorizados</li>
              <li>Interromper ou sobrecarregar a infraestrutura da Plataforma</li>
              <li>Tentar acessar contas ou dados de outros usuários sem autorização</li>
              <li>Transmitir vírus, malware ou código malicioso</li>
              <li>Violar direitos de propriedade intelectual de terceiros</li>
              <li>Enviar spam ou mensagens não solicitadas em massa</li>
            </ul>
          </section>

          <section>
            <h2 className="text-xl font-bold text-foreground mb-3">6. Propriedade Intelectual</h2>
            <p className="text-muted-foreground leading-relaxed">
              Todo o conteúdo, código-fonte, design, marcas registradas e demais elementos da Plataforma são de propriedade exclusiva da Severina AI ou de seus licenciadores. Você recebe uma licença limitada, não exclusiva e revogável para utilizar a Plataforma conforme estes Termos.
            </p>
          </section>

          <section>
            <h2 className="text-xl font-bold text-foreground mb-3">7. Dados do Usuário</h2>
            <p className="text-muted-foreground leading-relaxed">
              A coleta, armazenamento e tratamento de dados pessoais são regidos pela nossa <Link href="/privacidade" className="text-primary hover:underline">Política de Privacidade</Link>. A Plataforma processa dados de clientes dos usuários (dados de terceiros) exclusivamente para prestação do serviço contratado, em conformidade com a LGPD.
            </p>
          </section>

          <section>
            <h2 className="text-xl font-bold text-foreground mb-3">8. Disponibilidade e Suporte</h2>
            <p className="text-muted-foreground leading-relaxed">
              A Severina AI compromete-se a manter disponibilidade de 99,9% da Plataforma. O suporte técnico é disponível via e-mail e chat durante horário comercial. Interrupções programadas para manutenção serão comunicadas com antecedência mínima de 48 horas.
            </p>
          </section>

          <section>
            <h2 className="text-xl font-bold text-foreground mb-3">9. Limitação de Responsabilidade</h2>
            <p className="text-muted-foreground leading-relaxed">
              A Severina AI não se responsabiliza por: (a) decisões tomadas com base nas informações fornecidas pela Plataforma; (b) interrupções causadas por provedores externos (WhatsApp, APIs de IA, serviços de nuvem); (c) perdas decorrentes de uso indevido da Plataforma; (d) dados incorretos inseridos pelos usuários.
            </p>
          </section>

          <section>
            <h2 className="text-xl font-bold text-foreground mb-3">10. Alterações nos Termos</h2>
            <p className="text-muted-foreground leading-relaxed">
              A Severina AI reserva-se o direito de alterar estes Termos a qualquer momento. Alterações significativas serão comunicadas por e-mail ou notificação na Plataforma com pelo menos 30 dias de antecedência. O uso continuado da Plataforma após as alterações constitui aceitação dos novos Termos.
            </p>
          </section>

          <section>
            <h2 className="text-xl font-bold text-foreground mb-3">11. Rescisão</h2>
            <p className="text-muted-foreground leading-relaxed">
              Você pode encerrar sua conta a qualquer momento through as configurações da Plataforma. A Severina AI pode suspender ou encerrar sua conta em caso de violação destes Termos, com notificação prévia. Após o encerramento, seus dados serão tratados conforme a Política de Privacidade.
            </p>
          </section>

          <section>
            <h2 className="text-xl font-bold text-foreground mb-3">12. Lei Aplicável e Foro</h2>
            <p className="text-muted-foreground leading-relaxed">
              Estes Termos são regidos pelas leis da República Federativa do Brasil. Fica eleito o foro da comarca de São Paulo/SP para dirimir quaisquer questões oriundas destes Termos, com renúncia expressa a qualquer outro, por mais privilegiado que seja.
            </p>
          </section>

          <section>
            <h2 className="text-xl font-bold text-foreground mb-3">13. Contato</h2>
            <p className="text-muted-foreground leading-relaxed">
              Em caso de dúvidas sobre estes Termos, entre em contato:
            </p>
            <ul className="list-disc list-inside text-muted-foreground space-y-2 mt-3">
              <li>E-mail: suporte@severina.ai</li>
              <li>WhatsApp: (11) 99999-0000</li>
            </ul>
          </section>
        </div>
      </div>
    </div>
  );
}
