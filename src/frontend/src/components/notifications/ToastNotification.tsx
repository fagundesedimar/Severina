'use client';

import { useEffect } from 'react';

interface Notification {
  id: string;
  type: string;
  title: string;
  message: string;
  timestamp: Date;
  read: boolean;
}

export default function ToastNotification({ notification, onDismiss }: { notification: Notification; onDismiss: () => void }) {
  useEffect(() => {
    const timer = setTimeout(onDismiss, 5000);
    return () => clearTimeout(timer);
  }, [onDismiss]);

  return (
    <div className="fixed top-20 right-4 bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-xl shadow-xl p-4 max-w-sm z-50 animate-slide-in">
      <div className="flex items-start gap-3">
        <div className="flex-shrink-0">
          <span className="material-symbols-outlined text-primary">notifications</span>
        </div>
        <div className="flex-1">
          <p className="font-semibold text-sm">{notification.title}</p>
          <p className="text-sm text-gray-600 dark:text-gray-400 mt-1">{notification.message}</p>
        </div>
        <button
          onClick={onDismiss}
          className="text-gray-400 hover:text-gray-600 text-lg"
        >
          ×
        </button>
      </div>
    </div>
  );
}
