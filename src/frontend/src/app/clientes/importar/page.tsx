'use client';

import { useState, useRef } from 'react';
import { useRouter } from 'next/navigation';
import type { ImportJob } from '@/types/client';
import { clientApi } from '@/services/clientApi';

export default function ImportarClientesPage() {
  const router = useRouter();
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [file, setFile] = useState<File | null>(null);
  const [isImporting, setIsImporting] = useState(false);
  const [result, setResult] = useState<ImportJob | null>(null);
  const [error, setError] = useState<string | null>(null);

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    const droppedFile = e.dataTransfer.files[0];
    if (droppedFile && droppedFile.name.endsWith('.csv')) {
      setFile(droppedFile);
      setError(null);
    } else {
      setError('Por favor, selecione um arquivo CSV');
    }
  };

  const handleFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    const selectedFile = e.target.files?.[0];
    if (selectedFile) {
      setFile(selectedFile);
      setError(null);
    }
  };

  const handleImport = async () => {
    if (!file) return;

    setIsImporting(true);
    setError(null);
    try {
      const job = await clientApi.importClients(file);
      setResult(job);
    } catch {
      setError('Erro ao importar arquivo. Verifique o formato e tente novamente.');
    } finally {
      setIsImporting(false);
    }
  };

  return (
    <div className="min-h-screen bg-surface">
      <header className="px-4 md:px-6 pt-4 md:pt-6 mb-6">
        <button
          onClick={() => router.back()}
          className="flex items-center gap-2 text-on-surface/60 hover:text-on-surface transition-colors mb-4"
        >
          <span className="material-symbols-outlined">arrow_back</span>
          <span className="text-sm">Voltar</span>
        </button>
        <h1 className="text-2xl md:text-3xl font-bold text-on-surface">Importar Clientes</h1>
        <p className="text-sm text-on-surface/60 mt-1">Importe sua base de clientes via arquivo CSV</p>
      </header>

      <main className="px-4 md:px-6 max-w-[800px] mx-auto space-y-6">
        {!result ? (
          <>
            {/* Upload Area */}
            <div
              onDrop={handleDrop}
              onDragOver={(e) => e.preventDefault()}
              onClick={() => fileInputRef.current?.click()}
              className="border-2 border-dashed border-outline-variant dark:border-outline rounded-xl p-12 text-center cursor-pointer hover:border-primary hover:bg-surface-container-low transition-all"
            >
              <input
                ref={fileInputRef}
                type="file"
                accept=".csv"
                onChange={handleFileSelect}
                className="hidden"
              />
              <span className="material-symbols-outlined text-5xl text-on-surface/30 block mb-4">
                upload_file
              </span>
              {file ? (
                <div>
                  <p className="text-sm font-medium text-on-surface">{file.name}</p>
                  <p className="text-xs text-on-surface/50 mt-1">
                    {(file.size / 1024).toFixed(1)} KB
                  </p>
                </div>
              ) : (
                <div>
                  <p className="text-sm text-on-surface/70">
                    Arraste um arquivo CSV aqui ou clique para selecionar
                  </p>
                  <p className="text-xs text-on-surface/40 mt-2">
                    Formato esperado: nome, email, telefone, empresa
                  </p>
                </div>
              )}
            </div>

            {error && (
              <div className="bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-xl p-4">
                <p className="text-sm text-red-600 dark:text-red-400">{error}</p>
              </div>
            )}

            <button
              onClick={handleImport}
              disabled={!file || isImporting}
              className="w-full py-3 bg-primary text-on-primary rounded-xl text-sm font-medium hover:bg-primary/90 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {isImporting ? (
                <span className="flex items-center justify-center gap-2">
                  <span className="material-symbols-outlined animate-spin text-lg">refresh</span>
                  Importando...
                </span>
              ) : (
                'Iniciar Importação'
              )}
            </button>

            {/* Instructions */}
            <div className="bg-surface-container-lowest dark:bg-surface-container border border-outline-variant dark:border-outline rounded-xl p-6">
              <h3 className="text-sm font-semibold text-on-surface mb-3">Formato do arquivo CSV</h3>
              <div className="bg-surface-container-low rounded-lg p-3 font-mono text-xs text-on-surface/70">
                nome,email,telefone,empresa<br />
                João Silva,joao@email.com,(11) 99999-1234,Silva LTDA<br />
                Maria Santos,maria@email.com,(11) 98888-5678,Santos ME
              </div>
              <ul className="mt-3 space-y-1 text-xs text-on-surface/50">
                <li>• Campos obrigatórios: nome</li>
                <li>• Delimitador: vírgula</li>
                <li>• Encoding: UTF-8</li>
                <li>• Máximo: 1000 linhas</li>
              </ul>
            </div>
          </>
        ) : (
          /* Result */
          <div className="bg-surface-container-lowest dark:bg-surface-container border border-outline-variant dark:border-outline rounded-xl p-6">
            <div className="text-center mb-6">
              <span className="material-symbols-outlined text-5xl text-emerald-500 block mb-3">
                check_circle
              </span>
              <h3 className="text-lg font-semibold text-on-surface">Importação Concluída</h3>
              <p className="text-sm text-on-surface/60 mt-1">{result.fileName}</p>
            </div>

            <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mb-6">
              <div className="text-center p-3 bg-surface-container-low rounded-lg">
                <p className="text-2xl font-bold text-on-surface">{result.totalRows}</p>
                <p className="text-xs text-on-surface/50">Total</p>
              </div>
              <div className="text-center p-3 bg-surface-container-low rounded-lg">
                <p className="text-2xl font-bold text-emerald-500">{result.importedRows}</p>
                <p className="text-xs text-on-surface/50">Importados</p>
              </div>
              <div className="text-center p-3 bg-surface-container-low rounded-lg">
                <p className="text-2xl font-bold text-amber-500">{result.skippedRows}</p>
                <p className="text-xs text-on-surface/50">Ignorados</p>
              </div>
              <div className="text-center p-3 bg-surface-container-low rounded-lg">
                <p className="text-2xl font-bold text-red-500">{result.errorRows}</p>
                <p className="text-xs text-on-surface/50">Erros</p>
              </div>
            </div>

            {result.errorMessage && (
              <div className="bg-red-50 dark:bg-red-900/20 rounded-lg p-3 mb-4">
                <p className="text-xs text-red-600 dark:text-red-400">{result.errorMessage}</p>
              </div>
            )}

            <button
              onClick={() => router.push('/clientes')}
              className="w-full py-3 bg-primary text-on-primary rounded-xl text-sm font-medium"
            >
              Ver Clientes Importados
            </button>
          </div>
        )}
      </main>
    </div>
  );
}
