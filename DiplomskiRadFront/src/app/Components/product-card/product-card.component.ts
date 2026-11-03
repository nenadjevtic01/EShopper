import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-product-card',
  templateUrl: './product-card.component.html',
  styleUrls: ['./product-card.component.scss'],
})
export class ProductCardComponent {
  @Input() product: any;
  @Input() discount: number | null = null;

  get picture(): any {
    const pictures = this.product?.picture;
    return Array.isArray(pictures) ? pictures[0] : pictures;
  }

  get secondPicture(): any {
    const pictures = this.product?.picture;
    return Array.isArray(pictures) && pictures.length > 1 ? pictures[1] : null;
  }

  get discountValue(): number {
    if (this.discount != null) {
      return this.discount;
    }

    const p = this.product;

    if (!p || p.oldPrice == null || p.oldPrice <= p.newPrice) {
      return 0;
    }

    return Math.round((1 - p.newPrice / p.oldPrice) * 100);
  }

  get stockLabel(): string {
    return this.discountValue > 0 ? 'Sold Out' : 'Not In Stock';
  }
}
