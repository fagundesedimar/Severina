'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import api from '@/services/api';

type TipoPessoa = 'Fisica' | 'Juridica';

export default function CadastroPage() {
  const router = useRouter();
  const [tipoPessoa, setTipoPessoa] = useState<TipoPessoa>('Fisica');
  const [nome, setNome] = useState('');
  const [documento, setDocumento] = useState('');
  const [email, setEmail] = useState('');
  const [telefone, setTelefone] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const formatCpf = (value: string) => {
    const digits = value.replace(/\D/g, '').slice(0, 11);
    return digits
      .replace(/(\d{3})(\d)/, '$1.$2')
      .replace(/(\d{3})(\d)/, '$1.$2')
      .replace(/(\d{3})(\d{1,2})$/, '$1-$2');
  };

  const formatCnpj = (value: string) => {
    const digits = value.replace(/\D/g, '').slice(0, 14);
    return digits
      .replace(/(\d{2})(\d)/, '$1.$2')
      .replace(/(\d{3})(\d)/, '$1.$2')
      .replace(/(\d{3})(\d)/, '$1/$2')
      .replace(/(\d{4})(\d{1,2})$/, '$1-$2');
  };

  const formatTelefone = (value: string) => {
    const digits = value.replace(/\D/g, '').slice(0, 11);
    if (digits.length <= 10) {
      return digits
        .replace(/(\d{2})(\d)/, '($1) $2')
        .replace(/(\d{4})(\d)/, '$1-$2');
    }
    return digits
      .replace(/(\d{2})(\d)/, '($1) $2')
      .replace(/(\d{5})(\d)/, '$1-$2');
  };

  const handleDocumentoChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const formatted = tipoPessoa === 'Fisica' ? formatCpf(e.target.value) : formatCnpj(e.target.value);
    setDocumento(formatted);
  };

  const handleTelefoneChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setTelefone(formatTelefone(e.target.value));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      const documentoDigits = documento.replace(/\D/g, '');
      
      await api.post('/api/v1/companies', {
        nome,
        cnpjCpf: documentoDigits,
        email,
        tipoPessoa: tipoPessoa === 'Fisica' ? 0 : 1,
        telefone: telefone.replace(/\D/g, '') || null,
      });

      router.push('/dashboard');
    } catch (err: unknown) {
      const axiosErr = err as { response?: { data?: { message?: string } } };
      setError(axiosErr.response?.data?.message || 'Erro ao cadastrar empresa');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-surface transition-colors duration-200 p-4">
      <div className="w-full max-w-lg p-8 bg-white dark:bg-gray-800 rounded-lg shadow-lg">
        <h1 className="text-2xl font-bold text-center mb-6 text-on-surface">
          Cadastro de Empresa
        </h1>

        <div className="flex mb-6 bg-gray-100 dark:bg-gray-700 rounded-lg p-1">
          <button
            type="button"
            onClick={() => setTipoPessoa('Fisica')}
            className={`flex-1 py-2 px-4 rounded-md text-sm font-medium transition-colors ${
              tipoPessoa === 'Fisica'
                ? 'bg-primary text-on-primary'
                : 'text-on-surface hover:bg-gray-200 dark:hover:bg-gray-600'
            }`}
            role="switch"
            aria-checked={tipoPessoa === 'Fisica'}
          >
            Pessoa Física
          </button>
          <button
            type="button"
            onClick={() => setTipoPessoa('Juridica')}
            className={`flex-1 py-2 px-4 rounded-md text-sm font-medium transition-colors ${
              tipoPessoa === 'Juridica'
                ? 'bg-primary text-on-primary'
                : 'text-on-surface hover:bg-gray-200 dark:hover:bg-gray-600'
            }`}
            role="switch"
            aria-checked={tipoPessoa === 'Juridica'}
          >
            Pessoa Jurídica
          </button>
        </div>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label htmlFor="nome" className="block text-sm font-medium mb-1 text-on-surface">
              {tipoPessoa === 'Fisica' ? 'Nome Completo' : 'Razão Social'}
            </label>
            <input
              type="text"
              id="nome"
              value={nome}
              onChange={(e) => setNome(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary"
              required
              aria-describedby="nome-help"
            />
            <p id="nome-help" className="text-xs text-on-surface/60 mt-1">
              {tipoPessoa === 'Fisica' ? 'Nome como consta no CPF' : 'Nome oficial da empresa'}
            </p>
          </div>

          <div>
            <label htmlFor="documento" className="block text-sm font-medium mb-1 text-on-surface">
              {tipoPessoa === 'Fisica' ? 'CPF' : 'CNPJ'}
            </label>
            <input
              type="text"
              id="documento"
              value={documento}
              onChange={handleDocumentoChange}
              placeholder={tipoPessoa === 'Fisica' ? '000.000.000-00' : '00.000.000/0000-00'}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary"
              required
              aria-describedby="documento-help"
            />
            <p id="documento-help" className="text-xs text-on-surface/60 mt-1">
              {tipoPessoa === 'Fisica' ? 'Digite apenas os números do CPF' : 'Digite apenas os números do CNPJ'}
            </p>
          </div>

          <div>
            <label htmlFor="email" className="block text-sm font-medium mb-1 text-on-surface">
              Email
            </label>
            <input
              type="email"
              id="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary"
              required
            />
          </div>

          <div>
            <label htmlFor="telefone" className="block text-sm font-medium mb-1 text-on-surface">
              Telefone
            </label>
            <input
              type="tel"
              id="telefone"
              value={telefone}
              onChange={handleTelefoneChange}
              placeholder="(00) 00000-0000"
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary"
              aria-describedby="telefone-help"
            />
            <p id="telefone-help" className="text-xs text-on-surface/60 mt-1">
              Opcional - WhatsApp ou telefone fixo
            </p>
          </div>

          {error && (
            <p className="text-red-500 text-sm" role="alert" aria-live="polite">{error}</p>
          )}

          <button
            type="submit"
            disabled={loading}
            className="w-full py-2 px-4 bg-primary text-on-primary rounded-md hover:bg-primary/90 disabled:opacity-50 transition-colors"
          >
            {loading ? 'Cadastrando...' : 'Cadastrar Empresa'}
          </button>
        </form>

        <p className="text-center mt-6 text-sm text-on-surface/60">
          Já tem uma conta?{' '}
          <a href="/login" className="text-primary hover:underline">
            Entrar
          </a>
        </p>
      </div>
    </div>
  );
}
