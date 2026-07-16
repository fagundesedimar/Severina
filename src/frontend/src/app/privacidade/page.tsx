import Link from "next/link";

export default function PrivacidadePage() {
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
          Política de Privacidade
        </h1>
        <p className="text-sm text-muted-foreground mb-8">
          Última atualização: Julho de 2026
        </p>

        <div className="prose prose-neutral dark:prose-invert max-w-none space-y-8 text-foreground">
          <section>
            <h2 className="text-xl font-bold text-foreground mb-3">1. Introdução</h2>
            <p className="text-muted-foreground leading-relaxed">
              A Severina AI (&quot;Plataforma&quot;, &quot;nós&quot;, &quot;nosso&quot;) é comprometida com a proteção da privacidade e dos dados pessoais dos usuários. Esta Política de Privacidade descreve como coletamos, utilizamos, armazenamos e protegemos suas informações, em conformidade com a Lei Geral de Proteção de Dados (LGPD - Lei nº 13.709/2018).
            </p>
          </section>

          <section>
            <h2 className="text-xl font-bold text-foreground mb-3">2. Dados Coletados</h2>
            <p className="text-muted-foreground leading-relaxed">
              Coletamos os seguintes tipos de dados:
            </p>
            <h3 className="text-base font-semibold text-foreground mt-4 mb-2">2.1 Dados de Cadastro</h3>
            <ul className="list-disc list-inside text-muted-foreground space-y-2">
              <li>Nome completo e e-mail do responsável pela empresa</li>
              <li>CNPJ/CPF da empresa</li>
              <li>Telefone e informações de contato</li>
              <li>Senha (armazenada com hash seguro)</li>
            </ul>
            <h3 className="text-base font-semibold text-foreground mt-4 mb-2">2.2 Dados de Clientes dos Usuários</h3>
            <ul className="list-disc list-inside text-muted-foreground space-y-2">
              <li>Nomes e contatos de clientes cadastrados na plataforma</li>
              <li>Histórico de atendimentos e conversas</li>
              <li>Informações de agendamentos e compromissos</li>
              <li>Dados financeiros (cobranças, pagamentos, inadimplência)</li>
            </ul>
            <h3 className="text-base font-semibold text-foreground mt-4 mb-2">2.3 Dados de Uso</h3>
            <ul className="list-disc list-inside text-muted-foreground space-y-2">
              <li>Logs de acesso e atividades na plataforma</li>
              <li>Informações do dispositivo e navegador</li>
              <li>Endereço IP e dados de localização aproximada</li>
            </ul>
          </section>

          <section>
            <h2 className="text-xl font-bold text-foreground mb-3">3. Finalidade do Tratamento</h2>
            <p className="text-muted-foreground leading-relaxed">
              Os dados são tratados para as seguintes finalidades:
            </p>
            <ul className="list-disc list-inside text-muted-foreground space-y-2 mt-3">
              <li><strong>Prestação do serviço:</strong> executar as funcionalidades da Plataforma (atendimento, agendamento, cobrança, CRM, analytics)</li>
              <li><strong>Autenticação e segurança:</strong> verificar identidade, prevenir fraudes e proteger contas</li>
              <li><strong>Comunicação:</strong> enviar notificações, lembretes, cobranças e atualizações do serviço</li>
              <li><strong>Melhoria do produto:</strong> analisar uso da plataforma para aprimorar funcionalidades</li>
              <li><strong>Conformidade legal:</strong> atender obrigações regulatórias e judiciais</li>
              <li><strong>Suporte técnico:</strong> responder solicitações e resolver problemas</li>
            </ul>
          </section>

          <section>
            <h2 className="text-xl font-bold text-foreground mb-3">4. Base Legal para o Tratamento</h2>
            <p className="text-muted-foreground leading-relaxed">
              O tratamento de dados é realizado com base em:
            </p>
            <ul className="list-disc list-inside text-muted-foreground space-y-2 mt-3">
              <li><strong>Execução de contrato</strong> (art. 7º, V da LGPD): para prestação dos serviços contratados</li>
              <li><strong>Legítimo interesse</strong> (art. 7º, IX da LGPD): para melhoria do produto e prevenção de fraudes</li>
              <li><strong>Consentimento</strong> (art. 7º, I da LGPD): para comunicações de marketing e cookies não essenciais</li>
              <li><strong>Obrigação legal</strong> (art. 7º, II da LGPD): para atender exigências regulatórias</li>
            </ul>
          </section>

          <section>
            <h2 className="text-xl font-bold text-foreground mb-3">5. Compartilhamento de Dados</h2>
            <p className="text-muted-foreground leading-relaxed">
              Seus dados podem ser compartilhados com:
            </p>
            <ul className="list-disc list-inside text-muted-foreground space-y-2 mt-3">
              <li><strong>Provedores de serviço:</strong> empresas que auxiliam na operação (hospedagem, APIs de IA, serviços de mensageria)</li>
              <li><strong>Integrações externas:</strong> WhatsApp Business API, gateways de pagamento (apenas dados necessários para a integração)</li>
              <li><strong>Autoridades competentes:</strong> quando exigido por lei ou ordem judicial</li>
            </ul>
            <p className="text-muted-foreground leading-relaxed mt-3">
              Todos os parceiros são obrigados contratualmente a manter a confidencialidade e segurança dos dados.
            </p>
          </section>

          <section>
            <h2 className="text-xl font-bold text-foreground mb-3">6. Segurança dos Dados</h2>
            <p className="text-muted-foreground leading-relaxed">
              Implementamos medidas técnicas e organizacionais para proteger seus dados:
            </p>
            <ul className="list-disc list-inside text-muted-foreground space-y-2 mt-3">
              <li>Criptografia de dados em trânsito (TLS 1.3) e em repouso (AES-256)</li>
              <li>Autenticação JWT com refresh token e suporte a MFA</li>
              <li>Isolamento lógico de dados por empresa (multi-tenant com company_id)</li>
              <li>Logs de auditoria para todas as operações sensíveis</li>
              <li>Backup automatizado com retenção de 30 dias</li>
              <li>Monitoramento contínuo de segurança e alertas em tempo real</li>
            </ul>
          </section>

          <section>
            <h2 className="text-xl font-bold text-foreground mb-3">7. Retenção de Dados</h2>
            <p className="text-muted-foreground leading-relaxed">
              Seus dados são mantidos enquanto sua conta estiver ativa. Após o encerramento da conta:
            </p>
            <ul className="list-disc list-inside text-muted-foreground space-y-2 mt-3">
              <li>Dados de cadastro: removidos em até 30 dias</li>
              <li>Dados de clientes e transações: mantidos por 5 anos para cumprimento de obrigações legais</li>
              <li>Logs de auditoria: mantidos por 2 anos</li>
              <li>Dados anonimizados podem ser mantidos indefinidamente para fins estatísticos</li>
            </ul>
          </section>

          <section>
            <h2 className="text-xl font-bold text-foreground mb-3">8. Direitos do Titular</h2>
            <p className="text-muted-foreground leading-relaxed">
              Conforme a LGPD, você tem direito a:
            </p>
            <ul className="list-disc list-inside text-muted-foreground space-y-2 mt-3">
              <li><strong>Confirmação</strong> da existência de tratamento de dados</li>
              <li><strong>Acesso</strong> aos dados pessoais tratados</li>
              <li><strong>Correção</strong> de dados incompletos ou desatualizados</li>
              <li><strong>Anonimização, bloqueio ou eliminação</strong> de dados desnecessários</li>
              <li><strong>Portabilidade</strong> dos dados a outro fornecedor</li>
              <li><strong>Eliminação</strong> dos dados tratados com consentimento</li>
              <li><strong>Informação</strong> sobre compartilhamento de dados</li>
              <li><strong>Revogação do consentimento</strong></li>
            </ul>
            <p className="text-muted-foreground leading-relaxed mt-3">
              Para exercer seus direitos, entre em contato via e-mail: privacidade@severina.ai
            </p>
          </section>

          <section>
            <h2 className="text-xl font-bold text-foreground mb-3">9. Dados de Terceiros (Clientes dos Usuários)</h2>
            <p className="text-muted-foreground leading-relaxed">
              A Plataforma processa dados de clientes dos usuários exclusivamente para prestação do serviço contratado. O usuário da Plataforma é o responsável pelos dados de seus clientes e deve obter o consentimento necessário conforme a LGPD. A Severina AI atua como operadora de dados nesse contexto.
            </p>
          </section>

          <section>
            <h2 className="text-xl font-bold text-foreground mb-3">10. Cookies</h2>
            <p className="text-muted-foreground leading-relaxed">
              Utilizamos cookies essenciais para o funcionamento da Plataforma (autenticação, preferências de tema). Não utilizamos cookies de rastreamento para fins de marketing sem consentimento prévio.
            </p>
          </section>

          <section>
            <h2 className="text-xl font-bold text-foreground mb-3">11. Transferência Internacional de Dados</h2>
            <p className="text-muted-foreground leading-relaxed">
              A infraestrutura da Plataforma está hospedada em provedores de nuvem com data centers no Brasil. Caso ocorra transferência internacional de dados, garantiremos que o destinatário esteja em país com grau de proteção adequado ou que existam garantias contratuais de proteção.
            </p>
          </section>

          <section>
            <h2 className="text-xl font-bold text-foreground mb-3">12. Menores de Idade</h2>
            <p className="text-muted-foreground leading-relaxed">
              A Plataforma não é destinada a menores de 18 anos. Não coletamos intencionalmente dados de menores de idade.
            </p>
          </section>

          <section>
            <h2 className="text-xl font-bold text-foreground mb-3">13. Alterações nesta Política</h2>
            <p className="text-muted-foreground leading-relaxed">
              Esta Política pode ser atualizada periodicamente. Alterações significativas serão comunicadas por e-mail ou notificação na Plataforma com pelo menos 30 dias de antecedência. Recomendamos a revisão periódica desta página.
            </p>
          </section>

          <section>
            <h2 className="text-xl font-bold text-foreground mb-3">14. Contato</h2>
            <p className="text-muted-foreground leading-relaxed">
              Para dúvidas sobre esta Política de Privacidade ou para exercer seus direitos:
            </p>
            <ul className="list-disc list-inside text-muted-foreground space-y-2 mt-3">
              <li>E-mail: privacidade@severina.ai</li>
              <li>Encarregado de Dados (DPO): dpo@severina.ai</li>
              <li>Canal de suporte: suporte@severina.ai</li>
            </ul>
          </section>
        </div>
      </div>
    </div>
  );
}
