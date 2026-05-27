import { useContext } from 'react';
import PedidoContext from '@/contexts/pedido';

export function usePedido() {
  return useContext(PedidoContext);
}
