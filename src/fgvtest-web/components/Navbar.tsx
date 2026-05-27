'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { cn } from '@/lib/utils';
import { ShoppingCart, Package, Users } from 'lucide-react';

const links = [
  { href: '/', label: 'Pedidos', icon: ShoppingCart },
  { href: '/produtos/cadastro', label: 'Produtos', icon: Package },
  { href: '/clientes/cadastro', label: 'Clientes', icon: Users },
];

export function Navbar() {
  const pathname = usePathname();
  return (
    <nav className="sticky top-0 z-40 border-b border-border bg-background/90 backdrop-blur-md">
      <div className="max-w-7xl mx-auto px-4 flex items-center gap-1 h-14">
        <div className="flex items-center gap-2.5 mr-8">
          <div className="w-7 h-7 rounded-lg bg-primary flex items-center justify-center shrink-0">
            <ShoppingCart size={13} className="text-primary-foreground" strokeWidth={2} />
          </div>
          <span className="font-semibold text-sm tracking-tight text-foreground">
            Sistema de Vendas
          </span>
        </div>
        {links.map(({ href, label, icon: Icon }) => (
          <Link
            key={href}
            href={href}
            className={cn(
              'flex items-center gap-1.5 px-3 py-1.5 rounded-md text-sm font-medium transition-all duration-150',
              pathname === href
                ? 'bg-primary/10 text-primary'
                : 'text-muted-foreground hover:text-foreground hover:bg-muted'
            )}
          >
            <Icon size={14} strokeWidth={2} />
            {label}
          </Link>
        ))}
      </div>
    </nav>
  );
}
