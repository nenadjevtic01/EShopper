import { Component, OnInit } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';
import { PromotionService } from '../../../Services/promotion.service';
import { ProductsService } from '../../../Services/products.service';
import { ToastService } from '../../../Services/toast.service';
import { ConfirmService } from '../../../Services/confirm.service';
import { withMinDuration } from '../../../Utils/loading.util';

interface PromotionGroupForm {
  discountPercentage: number;
  selectedProductIds: number[];
  search: string;
}

@Component({
  selector: 'app-admin-promotions',
  templateUrl: './admin-promotions.component.html',
  styleUrls: ['./admin-promotions.component.scss'],
})
export class AdminPromotionsComponent implements OnInit {
  headers: any;
  promotions: any[] = [];
  dataLoaded: boolean = false;
  loading: boolean = false;
  includeInactive: boolean = false;
  keyword: string = '';

  allProducts: any[] = [];

  expandedPromotionIds: Set<number> = new Set<number>();

  showCreateForm: boolean = false;
  creating: boolean = false;
  createName: string = '';
  createValidFrom: string = '';
  createValidUntil: string = '';
  createGroups: PromotionGroupForm[] = [];

  editingPromotionId: number | null = null;
  saving: boolean = false;
  editName: string = '';
  editValidFrom: string = '';
  editValidUntil: string = '';
  editGroups: PromotionGroupForm[] = [];

  constructor(
    private promotionService: PromotionService,
    private productsService: ProductsService,
    private toastService: ToastService,
    private confirmService: ConfirmService,
  ) {}

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    this.headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    this.productsService.getAllForPicker(this.headers).subscribe({
      next: (data: any) => {
        this.allProducts = data.data;
      },
      error: () => {
        this.toastService.error('Could not load products.');
      },
    });

    this.loadPromotions();
  }

  loadPromotions(): void {
    this.loading = true;

    withMinDuration(
      this.promotionService.search(
        this.keyword || undefined,
        this.includeInactive,
        this.headers,
      ),
    ).subscribe({
      next: (data: any) => {
        this.promotions = data.data;
        this.dataLoaded = true;
        this.loading = false;
      },
      error: () => {
        this.dataLoaded = true;
        this.loading = false;
        this.toastService.error('Could not load promotions.');
      },
    });
  }

  search(): void {
    this.loadPromotions();
  }

  toggleExpand(promotionId: number): void {
    if (this.expandedPromotionIds.has(promotionId)) {
      this.expandedPromotionIds.delete(promotionId);
    } else {
      this.expandedPromotionIds.add(promotionId);
    }
  }

  private emptyGroup(): PromotionGroupForm {
    return { discountPercentage: 10, selectedProductIds: [], search: '' };
  }

  private toDateInputValue(date: Date): string {
    return date.toISOString().slice(0, 10);
  }

  status(promo: any): 'expired' | 'inactive' | 'active' | 'scheduled' {
    const today = this.toDateInputValue(new Date());
    const from = (promo.validFrom || '').slice(0, 10);
    const until = (promo.validUntil || '').slice(0, 10);

    if (until < today) {
      return 'expired';
    }

    if (!promo.isActive) {
      return 'inactive';
    }

    return from > today ? 'scheduled' : 'active';
  }

  statusLabel(promo: any): string {
    const labels: any = {
      expired: 'Expired',
      inactive: 'Inactive',
      active: 'Active',
      scheduled: 'Scheduled',
    };
    return labels[this.status(promo)];
  }

  statusClass(promo: any): string {
    const classes: any = {
      expired: 'badge-dark',
      inactive: 'badge-secondary',
      active: 'badge-success',
      scheduled: 'badge-info',
    };

    return classes[this.status(promo)];
  }

  toggleCreateForm(): void {
    this.showCreateForm = !this.showCreateForm;

    if (this.showCreateForm) {
      this.editingPromotionId = null;
      this.createName = '';
      const today = new Date();
      const in30Days = new Date();
      in30Days.setDate(in30Days.getDate() + 30);
      this.createValidFrom = this.toDateInputValue(today);
      this.createValidUntil = this.toDateInputValue(in30Days);
      this.createGroups = [this.emptyGroup()];
    }
  }

  addGroup(groups: PromotionGroupForm[]): void {
    groups.push(this.emptyGroup());
  }

  removeGroup(groups: PromotionGroupForm[], index: number): void {
    groups.splice(index, 1);
  }

  toggleGroupProduct(group: PromotionGroupForm, productId: number): void {
    const idx = group.selectedProductIds.indexOf(productId);

    if (idx > -1) {
      group.selectedProductIds.splice(idx, 1);
    } else {
      group.selectedProductIds.push(productId);
    }
  }

  filteredProducts(group: PromotionGroupForm): any[] {
    const term = (group.search || '').toLowerCase();

    return this.allProducts.filter((p: any) => {
      const matchesSearch = !term || p.productName.toLowerCase().includes(term);
      const selectable =
        (p.inStock && p.isActive !== false) ||
        group.selectedProductIds.includes(p.id);
      return matchesSearch && selectable;
    });
  }

  private buildGroupsPayload(groups: PromotionGroupForm[]): any[] {
    return groups
      .filter((g) => g.selectedProductIds.length > 0)
      .map((g) => ({
        DiscountPercentage: g.discountPercentage,
        ProductIds: g.selectedProductIds,
      }));
  }

  createPromotion(): void {
    if (!this.createName.trim()) {
      this.toastService.error('Promotion name is required.');
      return;
    }

    if (!this.createValidFrom || !this.createValidUntil) {
      this.toastService.error('Valid from and valid until dates are required.');
      return;
    }

    if (this.createValidFrom > this.createValidUntil) {
      this.toastService.error(
        "Valid until date can't be before the valid from date.",
      );
      return;
    }

    if (this.createValidUntil < this.toDateInputValue(new Date())) {
      this.toastService.error("Valid until date can't be in the past.");
      return;
    }

    const groupsPayload = this.buildGroupsPayload(this.createGroups);

    if (groupsPayload.length === 0) {
      this.toastService.error('Add at least one product to a discount group.');
      return;
    }

    this.creating = true;

    this.promotionService
      .create(
        {
          Name: this.createName,
          ValidFrom: this.createValidFrom,
          ValidUntil: this.createValidUntil,
          Groups: groupsPayload,
        },
        this.headers,
      )
      .subscribe({
        next: () => {
          this.creating = false;
          this.showCreateForm = false;
          this.toastService.success('Promotion created.');
          this.loadPromotions();
        },
        error: (err: any) => {
          this.creating = false;

          this.toastService.error(
            err?.error?.errors?.[0]?.error ||
              err?.error?.message ||
              'Could not create promotion.',
          );
        },
      });
  }

  startEdit(promotion: any): void {
    this.showCreateForm = false;
    this.editingPromotionId = promotion.id;
    this.editName = promotion.name;
    this.editValidFrom = (promotion.validFrom || '').slice(0, 10);
    this.editValidUntil = (promotion.validUntil || '').slice(0, 10);

    this.editGroups = promotion.groups.map((g: any) => ({
      discountPercentage: g.discountPercentage,
      selectedProductIds: g.products.map((p: any) => p.productId),
      search: '',
    }));

    if (this.editGroups.length === 0) {
      this.editGroups = [this.emptyGroup()];
    }
  }

  cancelEdit(): void {
    this.editingPromotionId = null;
  }

  saveEdit(): void {
    if (!this.editName.trim()) {
      this.toastService.error('Promotion name is required.');
      return;
    }

    if (!this.editValidFrom || !this.editValidUntil) {
      this.toastService.error('Valid from and valid until dates are required.');
      return;
    }

    if (this.editValidFrom > this.editValidUntil) {
      this.toastService.error(
        "Valid until date can't be before the valid from date.",
      );
      return;
    }

    const groupsPayload = this.buildGroupsPayload(this.editGroups);

    if (groupsPayload.length === 0) {
      this.toastService.error('Add at least one product to a discount group.');
      return;
    }

    this.saving = true;

    this.promotionService
      .updatePromotion(
        this.editingPromotionId as number,
        {
          Name: this.editName,
          ValidFrom: this.editValidFrom,
          ValidUntil: this.editValidUntil,
          Groups: groupsPayload,
        },
        this.headers,
      )
      .subscribe({
        next: () => {
          this.saving = false;
          this.editingPromotionId = null;
          this.toastService.success('Promotion updated.');
          this.loadPromotions();
        },
        error: (err: any) => {
          this.saving = false;

          this.toastService.error(
            err?.error?.errors?.[0]?.error ||
              err?.error?.message ||
              'Could not update promotion.',
          );
        },
      });
  }

  async removePromotion(id: number, name: string): Promise<void> {
    const confirmed = await this.confirmService.confirm(
      `Delete promotion "${name}"? Its scheduled prices will be removed and the products will go back to their regular prices.`,
      'Delete promotion',
      'Delete',
      true,
    );

    if (!confirmed) {
      return;
    }

    this.promotionService.delete(id, this.headers).subscribe({
      next: () => {
        this.toastService.success('Promotion deleted.');
        this.loadPromotions();
      },
      error: (err: any) => {
        this.toastService.error(
          err?.error?.errors?.[0]?.error ||
            err?.error?.message ||
            'Could not delete promotion.',
        );
      },
    });
  }

  async togglePromotion(
    id: number,
    name: string,
    isActive: boolean,
  ): Promise<void> {
    const message = isActive
      ? `Deactivate promotion "${name}"? Its products will go back to their regular prices.`
      : `Reactivate promotion "${name}"? Its products will be discounted again during its dates.`;

    const confirmed = await this.confirmService.confirm(
      message,
      isActive ? 'Deactivate promotion' : 'Reactivate promotion',
      isActive ? 'Deactivate' : 'Reactivate',
      isActive,
    );

    if (!confirmed) {
      return;
    }

    this.promotionService.toggle(id, this.headers).subscribe({
      next: () => {
        this.toastService.success(
          isActive ? 'Promotion deactivated.' : 'Promotion reactivated.',
        );
        this.loadPromotions();
      },
      error: (err: any) => {
        this.toastService.error(
          err?.error?.errors?.[0]?.error ||
            err?.error?.message ||
            'Could not update promotion.',
        );
      },
    });
  }
}
