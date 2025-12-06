<template>
  <svg xmlns="http://www.w3.org/2000/svg" width="80" height="80" viewBox="0 0 80 80" fill="none">
    <!-- Tête -->
    <circle cx="40" cy="30" r="18" fill="#f97316" stroke="#ea580c" stroke-width="2"/>
    <!-- Yeux -->
    <circle cx="35" cy="28" r="2" fill="#1e293b"/>
    <circle cx="45" cy="28" r="2" fill="#1e293b"/>
    <!-- Bouche -->
    <path d="M35 35 Q40 38 45 35" stroke="#1e293b" stroke-width="2" fill="none" stroke-linecap="round"/>
    <!-- Corps (uniforme de prison) -->
    <rect x="28" y="48" width="24" height="28" rx="2" fill="#475569" stroke="#334155" stroke-width="2"/>
    <!-- Numéro de prisonnier -->
    <text x="40" y="68" font-family="monospace" font-size="12" font-weight="bold" fill="#f97316" text-anchor="middle">001</text>
    <!-- Chaînes (optionnel) -->
    <circle cx="20" cy="50" r="3" fill="#64748b"/>
    <circle cx="60" cy="50" r="3" fill="#64748b"/>
    <path d="M20 50 L28 55" stroke="#64748b" stroke-width="2" stroke-linecap="round"/>
    <path d="M60 50 L52 55" stroke="#64748b" stroke-width="2" stroke-linecap="round"/>
  </svg>
</template>

