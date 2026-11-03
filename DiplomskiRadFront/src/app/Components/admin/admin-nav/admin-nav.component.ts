import { Component, OnDestroy } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { Subscription, filter } from 'rxjs';

interface AdminNavLink {
  route: string;
  icon: string;
  label: string;
}

interface AdminNavGroup {
  label: string;
  icon: string;
  links: AdminNavLink[];
}

@Component({
  selector: 'app-admin-nav',
  templateUrl: './admin-nav.component.html',
  styleUrls: ['./admin-nav.component.scss'],
})
export class AdminNavComponent implements OnDestroy {
  groups: AdminNavGroup[] = [
    {
      label: 'General',
      icon: 'fa-cog',
      links: [
        { route: '/admin/users', icon: 'fa-users', label: 'Users' },
        { route: '/admin/orders', icon: 'fa-receipt', label: 'Orders' },
        { route: '/admin/stats', icon: 'fa-chart-pie', label: 'Insights' },
        { route: '/admin/messages', icon: 'fa-envelope', label: 'Messages' },
        {
          route: '/admin/newsletter',
          icon: 'fa-paper-plane',
          label: 'Newsletter',
        },
        {
          route: '/admin/payment-methods',
          icon: 'fa-credit-card',
          label: 'Payment Methods',
        },
        { route: '/admin/hero', icon: 'fa-images', label: 'Home Hero' },
        {
          route: '/admin/audit-logs',
          icon: 'fa-clipboard-list',
          label: 'Audit Log',
        },
      ],
    },
    {
      label: 'Store',
      icon: 'fa-tshirt',
      links: [
        { route: '/admin/products', icon: 'fa-tshirt', label: 'Products' },
        { route: '/admin/catalog', icon: 'fa-list', label: 'Catalog' },
        { route: '/admin/coupons', icon: 'fa-percent', label: 'Coupons' },
        { route: '/admin/promotions', icon: 'fa-tags', label: 'Promotions' },
      ],
    },
  ];

  activeGroupIndex: number = 0;
  private routerSubscription: Subscription;

  constructor(private router: Router) {
    this.updateActiveGroup();

    this.routerSubscription = this.router.events
      .pipe(filter((e) => e instanceof NavigationEnd))
      .subscribe(() => {
        this.updateActiveGroup();
      });
  }

  ngOnDestroy(): void {
    this.routerSubscription.unsubscribe();
  }

  private updateActiveGroup(): void {
    const url = this.router.url;
    const idx = this.groups.findIndex((g) =>
      g.links.some((l) => url.startsWith(l.route)),
    );

    if (idx !== -1) {
      this.activeGroupIndex = idx;
    }
  }

  selectGroup(index: number): void {
    this.activeGroupIndex = index;
    const currentlyInGroup = this.groups[index].links.some((l) =>
      this.router.url.startsWith(l.route),
    );

    if (!currentlyInGroup) {
      this.router.navigate([this.groups[index].links[0].route]);
    }
  }
}
