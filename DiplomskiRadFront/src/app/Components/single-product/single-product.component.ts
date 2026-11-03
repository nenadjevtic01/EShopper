import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { IBrand } from '../../Interfaces/i-brand';
import { ICategory } from '../../Interfaces/i-category';
import { IGender } from '../../Interfaces/i-gender';
import { CartService } from '../../Services/cart.service';
import { HttpHeaders } from '@angular/common/http';
import { SingleProductService } from '../../Services/single-product.service';
import { ProductsService } from '../../Services/products.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { ToastService } from '../../Services/toast.service';
import { GuestCartService } from '../../Services/guest-cart.service';

@Component({
  selector: 'app-single-product',
  templateUrl: './single-product.component.html',
  styleUrls: ['./single-product.component.scss'],
})
export class SingleProductComponent implements OnInit {
  activeTab: 'info' | 'reviews' = 'info';
  cart: any;
  product: any;
  productId: number = 0;
  brand: IBrand[] = [];
  productBrand: IBrand[] = [];
  category: ICategory[] = [];
  productCategory: ICategory[] = [];
  randomProducts: any = [];
  gender: IGender[] = [];
  productGender: IGender[] = [];
  dataLoaded: boolean = false;
  selectedSize: number = 0;
  quantity: number = 1;
  currentImageIndex: number = 0;
  addToCartDisabled: boolean = true;
  reviewsVisibleCount: number = 3;

  constructor(
    private jwtHelper: JwtHelperService,
    public cartService: CartService,
    private route: ActivatedRoute,
    private router: Router,
    private singleProductService: SingleProductService,
    private productService: ProductsService,
    private toastService: ToastService,
    private guestCartService: GuestCartService,
  ) {}

  ngOnInit() {
    window.scrollTo({ top: 0 });
    this.brand = this.route.snapshot.data['brand'];
    this.category = this.route.snapshot.data['category'];
    this.gender = this.route.snapshot.data['gender'];

    this.route.paramMap.subscribe((param) => {
      this.productId = Number(param.get('id'));
    });

    this.singleProductService.get(this.productId).subscribe({
      next: (data: any) => {
        this.product = data;
        this.currentImageIndex = 0;
        let brandId = this.product.brand;
        let categoryId = this.product.category;
        let genderId = this.product.gender;
        this.getBrandName(brandId);
        this.getCategoryName(categoryId);
        this.getGenderName(genderId);
        this.dataLoaded = true;
      },
      error: () => {
        this.router.navigate(['/']);
      },
    });

    this.productService.search({ sortBy: 'random', perPage: 9 }).subscribe({
      next: (data: any) => {
        this.randomProducts = data.data
          .filter((p: any) => p.id !== this.productId)
          .slice(0, 8);
      },
      error: () => {
        this.router.navigate(['/']);
      },
    });
  }

  addToCart(product: number, size: number, quantity: number): void {
    if (size <= 0 || quantity < 1) {
      if (size <= 0) {
        this.toastService.error('Please select a size.');
      }

      if (quantity < 1 || quantity > 99) {
        this.toastService.error('Invalid quantity.');
      }

      return;
    }

    if (this.isAdmin()) {
      this.toastService.error("Admins can't add items to a cart.");
      return;
    }

    let token: string | null = localStorage.getItem('token');

    if (!token) {
      const selected = this.product.sizes.find((s: any) => s.id == size);

      this.guestCartService.addItem({
        productId: product,
        productName: this.product.productName,
        picture: {
          src: this.product.pictures[0]?.src,
          alt: this.product.pictures[0]?.alt,
        },
        sizeId: size,
        size: selected ? selected.name : '',
        quantity: quantity,
        unitPrice: this.product.newPrice,
      });

      this.toastService.success('Added to cart!');
      return;
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    if (token) {
      const decodedToken = this.jwtHelper.decodeToken(token);
      let userId = decodedToken.UserId;

      this.cartService.get(userId, headers).subscribe({
        next: (data: any) => {
          this.cart = data;
        },
        error: () => {},
      });

      setTimeout(() => {
        if (size > 0 && quantity >= 1) {
          if (this.cart != null) {
            const alreadyExist = this.cart.items.find((item: any) => {
              return (
                item.cartId == this.cart.cartId &&
                item.productId == product &&
                item.sizeId == size
              );
            });

            if (alreadyExist) {
              let cartItemId = alreadyExist.cartItemId;
              let id = this.cart.cartId;

              let dataToSend = {
                cartItemId: cartItemId,
                cartId: id,
                productId: product,
                sizeId: size,
                quantity: quantity,
              };

              this.cartService.update(dataToSend, headers).subscribe({
                next: () => {
                  this.cartService.get(userId, headers).subscribe();
                  this.toastService.success('Cart updated!');
                },
                error: (err: any) => {
                  this.toastService.errorFrom(err, 'Could not update cart.');
                },
              });
            } else {
              let dataToSend = {
                cartId: this.cart.cartId,
                productId: product,
                sizeId: size,
                quantity: quantity,
              };

              this.cartService.create(dataToSend, headers).subscribe({
                next: () => {
                  this.cartService.get(userId, headers).subscribe();
                  this.toastService.success('Added to cart!');
                },
                error: (err: any) => {
                  this.toastService.errorFrom(
                    err,
                    'Could not add item to cart.',
                  );
                },
              });
            }
          }
        } else {
          if (size == 0) {
            this.toastService.error('Please select a size.');
          }

          if (quantity < 1 || quantity > 99) {
            this.toastService.error('Invalid quantity.');
          }
        }
      }, 1000);
    }
  }

  roundedRating(value: number): number {
    return Math.ceil((value || 0) * 2) / 2;
  }

  get discountPercent(): number {
    if (
      !this.product ||
      this.product.oldPrice == null ||
      this.product.oldPrice <= this.product.newPrice
    ) {
      return 0;
    }

    return Math.round(
      (1 - this.product.newPrice / this.product.oldPrice) * 100,
    );
  }

  showMoreReviews(): void {
    this.reviewsVisibleCount += 5;
  }

  get visibleReviews(): any[] {
    return (this.product?.ratings || []).slice(0, this.reviewsVisibleCount);
  }

  starClass(star: number, rating: number): string {
    const rounded = this.roundedRating(rating);

    if (star <= Math.floor(rounded)) {
      return 'fas fa-star';
    }

    if (star - rounded === 0.5) {
      return 'fas fa-star-half-alt';
    }

    return 'far fa-star';
  }

  isAdmin(): boolean {
    let token: string | null = localStorage.getItem('token');

    if (!token) {
      return false;
    }

    const decodedToken = this.jwtHelper.decodeToken(token);
    return Number(decodedToken.Role) === 1;
  }

  getBrandName(id: number) {
    this.productBrand = this.brand.filter((x) => x.id == id);
  }

  getCategoryName(id: number) {
    this.productCategory = this.category.filter((x) => x.id == id);
  }

  getGenderName(id: number) {
    this.productGender = this.gender.filter((x) => x.id == id);
  }

  get selectedSizeStock(): number {
    if (!this.selectedSize || !this.product?.sizes) {
      return 0;
    }

    const size = this.product.sizes.find(
      (s: any) => s.id === this.selectedSize,
    );
    return size ? size.quantity : 0;
  }

  decreaseQuantity() {
    if (this.quantity > 1) {
      this.quantity--;
    }
  }

  increaseQuantity() {
    if (!this.selectedSize || this.quantity < this.selectedSizeStock) {
      this.quantity++;
    }
  }

  onSizeChange() {
    if (this.quantity > this.selectedSizeStock) {
      this.quantity = this.selectedSizeStock || 1;
    }
  }

  checkDisabled() {
    this.addToCartDisabled =
      !this.selectedSize ||
      this.quantity < 1 ||
      this.quantity > this.selectedSizeStock;
  }

  nextImage(): void {
    if (!this.product?.pictures?.length) {
      return;
    }

    this.currentImageIndex =
      (this.currentImageIndex + 1) % this.product.pictures.length;
  }

  previousImage(): void {
    if (!this.product?.pictures?.length) {
      return;
    }

    this.currentImageIndex =
      (this.currentImageIndex - 1 + this.product.pictures.length) %
      this.product.pictures.length;
  }

  goToImage(index: number): void {
    this.currentImageIndex = index;
  }
}
