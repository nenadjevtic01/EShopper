import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { IBrand } from '../../Interfaces/i-brand';
import { ICategory } from '../../Interfaces/i-category';
import { ProductsService } from '../../Services/products.service';
import { CategoryService } from '../../Services/category.service';
import { NewsletterService } from '../../Services/newsletter.service';
import { ToastService } from '../../Services/toast.service';
import { PromotionService } from '../../Services/promotion.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  brands: IBrand[] = [];
  categories: ICategory[] = [];
  activePromotions: any[] = [];
  recentProducts: any = [];
  newsletterEmail: string = '';
  subscribing: boolean = false;

  readonly perks = [
    {
      icon: 'fa-gem',
      title: 'Quality products',
      text: 'Carefully selected fabrics and finishes.',
    },
    {
      icon: 'fa-shipping-fast',
      title: 'Delivery to your door',
      text: 'Shipping cost is calculated at checkout.',
    },
    {
      icon: 'fa-undo-alt',
      title: '14-day returns',
      text: 'Changed your mind? Send it back.',
    },
    {
      icon: 'fa-headset',
      title: '24/7 support',
      text: 'We are always a message away.',
    },
  ];

  constructor(
    private route: ActivatedRoute,
    private productService: ProductsService,
    private categoryService: CategoryService,
    private newsletterService: NewsletterService,
    private toastService: ToastService,
    private promotionService: PromotionService,
  ) {}

  ngOnInit(): void {
    window.scrollTo({ top: 0 });

    this.productService
      .search({ sortBy: 'updated', sortDirection: 'desc', perPage: 8 })
      .subscribe({
        next: (data: any) => {
          this.recentProducts = data.data;
        },
        error: () => {},
      });

    this.promotionService.getActive().subscribe({
      next: (data: any) => {
        this.activePromotions = data;
      },
      error: () => {},
    });

    this.categoryService.getCategory().subscribe({
      next: (data: ICategory[]) => {
        this.categories = data;
      },
      error: () => {},
    });

    this.brands = this.route.snapshot.data['brand'];
  }

  subscribe(): void {
    if (
      !this.newsletterEmail ||
      !/^[^@\s]+@[^@\s]+\.[^@\s]+$/.test(this.newsletterEmail)
    ) {
      this.toastService.error('Please enter a valid e-mail address.');
      return;
    }

    if (this.newsletterEmail.length > 100) {
      this.toastService.error('E-mail can have at most 100 characters.');
      return;
    }

    this.subscribing = true;

    this.newsletterService.subscribe(this.newsletterEmail).subscribe({
      next: () => {
        this.subscribing = false;
        this.toastService.success('Thanks for subscribing!');
        this.newsletterEmail = '';
      },
      error: () => {
        this.subscribing = false;
        this.toastService.error('Could not subscribe. Please try again.');
      },
    });
  }
}
