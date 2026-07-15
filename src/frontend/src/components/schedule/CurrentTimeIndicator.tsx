'use client';

import { useEffect, useState } from 'react';

interface CurrentTimeIndicatorProps {
  startHour: number;
  endHour: number;
}

export default function CurrentTimeIndicator({ startHour, endHour }: CurrentTimeIndicatorProps) {
  const [, setCurrentTime] = useState(new Date());

  useEffect(() => {
    const interval = setInterval(() => {
      setCurrentTime(new Date());
    }, 60000);

    return () => clearInterval(interval);
  }, []);

  const now = new Date();
  const currentHour = now.getHours();
  const currentMinute = now.getMinutes();

  if (currentHour < startHour || currentHour >= endHour) {
    return null;
  }

  const hourHeight = 60;
  const topOffset = (currentHour - startHour) * hourHeight + (currentMinute / 60) * hourHeight;

  return (
    <div
      className="absolute left-[60px] right-0 h-0.5 bg-red-500 z-10"
      style={{ top: `${topOffset}px` }}
    >
      <div className="absolute -left-1.5 -top-1.5 w-3 h-3 bg-red-500 rounded-full" />
    </div>
  );
}
