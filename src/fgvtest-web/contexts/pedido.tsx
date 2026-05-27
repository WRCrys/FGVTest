'use client';

import { createContext, useContext, useState, ReactNode } from 'react';
import { Pedido } from '@/types/pedido';

interface PedidoContextData {
  pedidoAtivo: Pedido | null;
  setPedidoAtivo: (pedido: Pedido | null) => void;
}

const PedidoContext = createContext<PedidoContextData>({} as PedidoContextData);

export function PedidoProvider({ children }: { children: ReactNode }) {
  const [pedidoAtivo, setPedidoAtivo] = useState<Pedido | null>(null);

  return (
    <PedidoContext.Provider value={{ pedidoAtivo, setPedidoAtivo }}>
      {children}
    </PedidoContext.Provider>
  );
}

export default PedidoContext;
