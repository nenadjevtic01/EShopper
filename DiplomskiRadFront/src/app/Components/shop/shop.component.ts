import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {
  Subject,
  Subscription,
  of,
  catchError,
  debounceTime,
  switchMap,
} from 'rxjs';
import { ICategory } from '../../Interfaces/i-category';
import { IFilter } from '../../Interfaces/i-filter';
import { IBrand } from '../../Interfaces/i-brand';
import { IGender } from '../../Interfaces/i-gender';
import { ISize } from '../../Interfaces/i-size';
import {
  ProductsService,
  IProductSearch,
} from '../../Services/products.service';
import { GenderService } from '../../Services/gender.service';
import { withMinDuration } from '../../Utils/loading.util';

@Component({
  selector: 'app-shop',
  templateUrl: './shop.component.html',
  styleUrls: ['./shop.component.scss'],
})
export class ShopComponent implements OnInit, OnDestroy {
  displayedProducts: any = [];
  totalCount: number = 0;
  pagesCount: number = 1;
  categoryChecked: string = '';
  brandChecked: string = '';
  genderChecked: string = '';
  categories: ICategory[] = [];
  brands: IBrand[] = [];
  genders: IGender[] = [];
  sizes: ISize[] = [];

  selectedFilters: IFilter = {
    sizes: [],
    brands: [],
    categories: [],
    genders: [],
    keyword: '',
    sortOption: 0,
  };

  productsPerPage: number = 15;
  currentPage: number = 1;
  keyword: string = '';
  sortOption: number = 0;
  dataLoaded: boolean = false;
  includeNotInStock: boolean = false;
  filtering: boolean = false;
  filtersOpen: boolean = false;

  private loadRequests = new Subject<void>();
  private keywordChanges = new Subject<string>();
  private subscriptions = new Subscription();

  constructor(
    private route: ActivatedRoute,
    private productService: ProductsService,
    public genderService: GenderService,
  ) {}

  ngOnInit(): void {
    window.scrollTo({ top: 0 });

    this.brands = this.route.snapshot.data['brand'];
    this.categories = this.route.snapshot.data['category'];
    this.genders = this.route.snapshot.data['gender'];
    this.sizes = this.route.snapshot.data['size'];

    this.subscriptions.add(
      this.loadRequests
        .pipe(
          switchMap(() => {
            this.filtering = true;

            return withMinDuration(
              this.productService.search(this.buildSearch()),
              300,
            ).pipe(catchError(() => of(null)));
          }),
        )
        .subscribe((response: any) => {
          if (response) {
            this.displayedProducts = response.data;
            this.totalCount = response.count;
            this.pagesCount = response.pagesCount;
          } else {
            this.displayedProducts = [];
            this.totalCount = 0;
            this.pagesCount = 1;
          }

          this.filtering = false;
          this.dataLoaded = true;
        }),
    );

    this.subscriptions.add(
      this.keywordChanges.pipe(debounceTime(300)).subscribe((value) => {
        this.selectedFilters.keyword = value;
        this.reload();
      }),
    );

    this.subscriptions.add(
      this.route.queryParams.subscribe((params) => {
        const category = params['category'];
        const brand = params['brand'];
        const gender = params['gender'];
        this.genderChecked = gender;
        this.brandChecked = brand;
        this.categoryChecked = category;

        if (category != null) {
          this.selectedFilters.categories = [parseInt(category)];
        }

        if (brand != null) {
          this.selectedFilters.brands = [parseInt(brand)];
        }

        if (gender != null) {
          this.selectedFilters.genders = [parseInt(gender)];
        }

        this.reload();
      }),
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  onKeyUp() {
    this.keywordChanges.next(this.keyword);
  }

  applySorting() {
    this.selectedFilters.sortOption = Number(this.sortOption);
    this.reload();
  }

  toggleFilter(filterType: string, value: any) {
    const selectedFilter = this.selectedFilters[filterType];

    if (Array.isArray(selectedFilter)) {
      const filterIndex = (selectedFilter as any[]).indexOf(value);

      if (filterIndex > -1) {
        (selectedFilter as any[]).splice(filterIndex, 1);
      } else {
        (selectedFilter as any[]).push(value);
      }
    } else {
      this.selectedFilters[filterType] = value;
    }

    this.reload();
  }

  onAvailabilityChange(): void {
    this.reload();
  }

  private reload(): void {
    this.currentPage = 1;
    this.loadRequests.next();
  }

  private buildSearch(): IProductSearch {
    let sortBy: string | undefined;
    let sortDirection: 'asc' | 'desc' | undefined;

    switch (Number(this.selectedFilters.sortOption)) {
      case 1:
        sortBy = 'price';
        sortDirection = 'asc';
        break;

      case 2:
        sortBy = 'price';
        sortDirection = 'desc';
        break;

      case 3:
        sortBy = 'name';
        sortDirection = 'asc';
        break;

      case 4:
        sortBy = 'name';
        sortDirection = 'desc';
        break;
    }

    return {
      keyword: this.selectedFilters.keyword,
      categoryIds: this.selectedFilters.categories,
      brandIds: this.selectedFilters.brands,
      genderIds: this.selectedFilters.genders,
      sizeIds: this.selectedFilters.sizes,
      onlyInStock: !this.includeNotInStock,
      sortBy: sortBy,
      sortDirection: sortDirection,
      page: this.currentPage,
      perPage: this.productsPerPage,
    };
  }

  onNextPage() {
    if (this.currentPage < this.totalPages()) {
      this.currentPage++;
      this.loadRequests.next();
    }

    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  onPreviousPage() {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadRequests.next();
    }

    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  totalPages() {
    return Math.max(this.pagesCount, 1);
  }

  discountPercent(p: any): number {
    if (!p || p.oldPrice == null || p.oldPrice <= p.newPrice) {
      return 0;
    }

    return Math.round((1 - p.newPrice / p.oldPrice) * 100);
  }

  protected readonly parseInt = parseInt;
}
