'use client';

import { useRouter } from 'next/navigation';
import type { ActivityDto } from '@/types/dashboard';

function formatTimeAgo(timestamp: string): string {
  const now = new Date();
  const date = new Date(timestamp);
  const diffMs = now.getTime() - date.getTime();
  const diffMin = Math.floor(diffMs / 60000);
  const diffHours = Math.floor(diffMs / 3600000);
  const diffDays = Math.floor(diffMs / 86400000);

  if (diffMin < 1) return 'agora';
  if (diffMin < 60) return `${diffMin}m atrás`;
  if (diffHours < 24) return `${diffHours}h atrás`;
  return `${diffDays}d atrás`;
}

function getActivityIcon(type: string): string {
  switch (type) {
    case 'appointment': return 'event';
    case 'client': return 'person';
    case 'payment': return 'payments';
    case 'message': return 'chat';
    default: return 'info';
  }
}

function getActivityColor(type: string): string {
  switch (type) {
    case 'appointment': return 'bg-blue-100 dark:bg-blue-900/30 text-blue-600 dark:text-blue-400';
    case 'client': return 'bg-green-100 dark:bg-green-900/30 text-green-600 dark:text-green-400';
    case 'payment': return 'bg-purple-100 dark:bg-purple-900/30 text-purple-600 dark:text-purple-400';
    case 'message': return 'bg-yellow-100 dark:bg-yellow-900/30 text-yellow-600 dark:text-yellow-400';
    default: return 'bg-gray-100 dark:bg-gray-700 text-gray-600 dark:text-gray-400';
  }
}

export function ActivityFeed({ activities }: { activities: ActivityDto[] }) {
  const router = useRouter();

  return (
    <div className="bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-xl p-5">
      <h3 className="text-sm font-semibold text-on-surface mb-4 flex items-center gap-2">
        <span className="material-symbols-outlined text-primary text-lg">history</span>
        Atividades Recentes
      </h3>
      {activities.length === 0 ? (
        <p className="text-sm text-gray-500 dark:text-gray-400 text-center py-8">
          Nenhuma atividade recente
        </p>
      ) : (
        <div className="space-y-3">
          {activities.map((activity) => (
            <div
              key={activity.id}
              onClick={() => activity.sourceUrl && router.push(activity.sourceUrl)}
              className={`flex items-start gap-3 p-3 rounded-lg transition-colors ${
                activity.sourceUrl
                  ? 'hover:bg-gray-50 dark:hover:bg-gray-700/50 cursor-pointer'
                  : ''
              }`}
            >
              <div className={`w-8 h-8 rounded-full flex items-center justify-center flex-shrink-0 ${getActivityColor(activity.type)}`}>
                <span className="material-symbols-outlined text-sm">{getActivityIcon(activity.type)}</span>
              </div>
              <div className="flex-1 min-w-0">
                <p className="text-sm text-on-surface truncate">{activity.description}</p>
                <p className="text-xs text-gray-500 dark:text-gray-400 mt-0.5">
                  {formatTimeAgo(activity.timestamp)}
                </p>
              </div>
              {activity.sourceUrl && (
                <span className="material-symbols-outlined text-gray-400 text-sm opacity-0 group-hover:opacity-100">
                  chevron_right
                </span>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
