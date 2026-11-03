import { Component, OnInit } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import {
  ProductsService,
  IProductSearch,
} from '../../../Services/products.service';
import { CategoryService } from '../../../Services/category.service';
import { BrandService } from '../../../Services/brand.service';
import { GenderService } from '../../../Services/gender.service';
import { SizeService } from '../../../Services/size.service';
import { CountryService } from '../../../Services/country.service';
import { ToastService } from '../../../Services/toast.service';
import { ConfirmService } from '../../../Services/confirm.service';
import { ICategory } from '../../../Interfaces/i-category';
import { IBrand } from '../../../Interfaces/i-brand';
import { IGender } from '../../../Interfaces/i-gender';
import { ISize } from '../../../Interfaces/i-size';
import { ICountry } from '../../../Interfaces/i-country';
import { withMinDuration } from '../../../Utils/loading.util';
import { UploadService } from '../../../Services/upload.service';
import { Observable, of, map, switchMap } from 'rxjs';

interface IPictureForm {
  src: string;
  alt: string;
  file?: File;
  uploaded?: string;
}

@Component({
  selector: 'app-admin-products',
  templateUrl: './admin-products.component.html',
  styleUrls: ['./admin-products.component.scss'],
})
export class AdminProductsComponent implements OnInit {
  headers: any;
  products: any[] = [];
  dataLoaded: boolean = false;
  loading: boolean = false;

  categories: ICategory[] = [];
  brands: IBrand[] = [];
  genders: IGender[] = [];
  sizes: ISize[] = [];
  countries: ICountry[] = [];

  filterCategoryId: number | null = null;
  filterBrandId: number | null = null;
  includeInactive: boolean = false;

  currentPage: number = 1;
  pagesCount: number = 1;
  readonly productsPerPage: number = 15;

  sortColumn: string | null = null;
  sortDirection: 'asc' | 'desc' = 'asc';

  readonly maxImages: number = 5;
  readonly allowedImageTypes: string[] = [
    'image/jpeg',
    'image/jpg',
    'image/png',
  ];

  showCreateForm: boolean = false;
  createForm!: FormGroup;
  createSelectedSizes: number[] = [];
  createSizeQuantities: { [sizeId: number]: number } = {};
  createPictures: IPictureForm[] = [];
  creating: boolean = false;

  editingProductId: number | null = null;
  editForm!: FormGroup;
  editSelectedSizes: number[] = [];
  editSizeQuantities: { [sizeId: number]: number } = {};
  editPictures: IPictureForm[] = [];
  editRegularPrice: number = 0;
  editOnSale: boolean = false;
  saving: boolean = false;

  readonly lowStockThreshold: number = 5;

  constructor(
    private productsService: ProductsService,
    private categoryService: CategoryService,
    private brandService: BrandService,
    private genderService: GenderService,
    private sizeService: SizeService,
    private countryService: CountryService,
    private toastService: ToastService,
    private confirmService: ConfirmService,
    private uploadService: UploadService,
  ) {}

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    this.headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    this.categoryService
      .getCategory()
      .subscribe((data) => (this.categories = data));
    this.brandService.getBrands().subscribe((data) => (this.brands = data));
    this.genderService.getGender().subscribe((data) => (this.genders = data));
    this.sizeService.getSizes().subscribe((data) => (this.sizes = data));
    this.countryService
      .getCountries(this.headers)
      .subscribe((data) => (this.countries = data));

    this.createForm = new FormGroup<any>({
      productName: new FormControl('', [
        Validators.required,
        Validators.minLength(5),
      ]),
      categoryId: new FormControl(null, [Validators.required]),
      brandId: new FormControl(null, [Validators.required]),
      genderId: new FormControl(null, [Validators.required]),
      sale: new FormControl(false),
      inStock: new FormControl(true),
      material: new FormControl('', [Validators.required]),
      countryId: new FormControl(null, [Validators.required]),
      price: new FormControl(null, [Validators.required]),
    });

    this.editForm = new FormGroup<any>({
      productId: new FormControl(0),
      productName: new FormControl('', [
        Validators.required,
        Validators.minLength(5),
      ]),
      categoryId: new FormControl(null, [Validators.required]),
      brandId: new FormControl(null, [Validators.required]),
      genderId: new FormControl(null, [Validators.required]),
      sale: new FormControl(false),
      inStock: new FormControl(true),
      material: new FormControl('', [Validators.required]),
      countryId: new FormControl(null, [Validators.required]),
      price: new FormControl(null, [
        Validators.required,
        Validators.min(0.01),
      ]),
    });

    this.createForm.get('inStock')?.disable();
    this.editForm.get('inStock')?.disable();
    this.createForm.get('sale')?.disable();
    this.editForm.get('sale')?.disable();

    this.loadProducts();
  }

  loadProducts(): void {
    this.loading = true;

    const sortColumns: { [column: string]: string } = {
      id: 'id',
      productName: 'name',
      newPrice: 'price',
      sale: 'sale',
      inStock: 'instock',
    };

    const search: IProductSearch = {
      categoryId: this.filterCategoryId || undefined,
      brandId: this.filterBrandId || undefined,
      includeInactive: this.includeInactive,
      sortBy: this.sortColumn ? sortColumns[this.sortColumn] : undefined,
      sortDirection: this.sortDirection,
      page: this.currentPage,
      perPage: this.productsPerPage,
    };

    withMinDuration(
      this.productsService.search(search, this.headers),
    ).subscribe({
      next: (data: any) => {
        this.pagesCount = Math.max(data.pagesCount, 1);

        if (data.data.length === 0 && this.currentPage > 1) {
          this.currentPage = this.pagesCount;
          this.loadProducts();
          return;
        }

        this.products = data.data;
        this.dataLoaded = true;
        this.loading = false;
      },
      error: () => {
        this.dataLoaded = true;
        this.loading = false;
        this.toastService.error('Could not load products.');
      },
    });
  }

  applyFilters(): void {
    this.currentPage = 1;
    this.loadProducts();
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadProducts();
    }
  }

  nextPage(): void {
    if (this.currentPage < this.pagesCount) {
      this.currentPage++;
      this.loadProducts();
    }
  }

  sortBy(column: string): void {
    if (this.sortColumn === column) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortColumn = column;
      this.sortDirection = 'asc';
    }

    this.currentPage = 1;
    this.loadProducts();
  }

  toggleCreateForm(): void {
    this.showCreateForm = !this.showCreateForm;
    this.discardPictures(this.createPictures);
    this.createPictures = [];

    if (this.showCreateForm) {
      this.editingProductId = null;
      this.createSelectedSizes = [];
      this.createSizeQuantities = {};
      this.createForm.get('inStock')?.setValue(false);
    }
  }

  private recomputeCreateInStock(): void {
    const inStock = this.createSelectedSizes.some(
      (id) => (this.createSizeQuantities[id] || 0) > 0,
    );
    this.createForm.get('inStock')?.setValue(inStock);
  }

  private recomputeEditInStock(): void {
    const inStock = this.editSelectedSizes.some(
      (id) => (this.editSizeQuantities[id] || 0) > 0,
    );
    this.editForm.get('inStock')?.setValue(inStock);
  }

  toggleCreateSize(sizeId: number): void {
    const index = this.createSelectedSizes.indexOf(sizeId);

    if (index > -1) {
      this.createSelectedSizes.splice(index, 1);
      delete this.createSizeQuantities[sizeId];
    } else {
      this.createSelectedSizes.push(sizeId);
      this.createSizeQuantities[sizeId] =
        this.createSizeQuantities[sizeId] || 0;
    }

    this.recomputeCreateInStock();
  }

  setCreateSizeQuantity(sizeId: number, quantity: number): void {
    this.createSizeQuantities[sizeId] = Math.max(0, quantity || 0);
    this.recomputeCreateInStock();
  }

  toggleEditSize(sizeId: number): void {
    const index = this.editSelectedSizes.indexOf(sizeId);

    if (index > -1) {
      this.editSelectedSizes.splice(index, 1);
      delete this.editSizeQuantities[sizeId];
    } else {
      this.editSelectedSizes.push(sizeId);
      this.editSizeQuantities[sizeId] = this.editSizeQuantities[sizeId] || 0;
    }

    this.recomputeEditInStock();
  }

  setEditSizeQuantity(sizeId: number, quantity: number): void {
    this.editSizeQuantities[sizeId] = Math.max(0, quantity || 0);
    this.recomputeEditInStock();
  }

  lowStockInfo(product: any): string | null {
    if (!product.sizeStocks || !product.sizeStocks.length) {
      return null;
    }

    const total = product.sizeStocks.reduce(
      (sum: number, s: any) => sum + (s.quantity || 0),
      0,
    );

    if (total === 0 || total > this.lowStockThreshold) {
      return null;
    }

    const perSize = product.sizeStocks
      .filter((s: any) => s.quantity > 0)
      .map((s: any) => `${s.name}: ${s.quantity}`)
      .join(', ');

    return `Low stock - only ${total} left (${perSize}).`;
  }

  onLowStockClick(event: Event, product: any): void {
    event.stopPropagation();
    const message = this.lowStockInfo(product);

    if (message) {
      this.toastService.error(message);
    }

    this.startEdit(product);
  }

  private categoryFolderName(categoryId: number | null): string | null {
    const category = this.categories.find((c) => c.id === categoryId);
    return category
      ? category.name.trim().toLowerCase().replace(/\s+/g, '-')
      : null;
  }

  private validateSelectedFiles(
    files: File[],
    existingCount: number,
  ): string | null {
    if (existingCount + files.length > this.maxImages) {
      return `You can have at most ${this.maxImages} images per product.`;
    }

    const invalidFile = files.find(
      (f) => !this.allowedImageTypes.includes(f.type),
    );

    if (invalidFile) {
      return `"${invalidFile.name}" is not a supported image type. Only JPG and PNG are allowed.`;
    }

    return null;
  }

  private pendingPictures(files: File[]): IPictureForm[] {
    return files.map((file) => ({
      src: URL.createObjectURL(file),
      alt: file.name.replace(/.[^/.]+$/, '') || 'Product image',
      file,
    }));
  }

  private releasePictures(pictures: IPictureForm[]): void {
    pictures.filter((p) => p.file).forEach((p) => URL.revokeObjectURL(p.src));
  }

  private discardPictures(pictures: IPictureForm[]): void {
    this.releasePictures(pictures);

    this.uploadService.discard(
      pictures
        .filter((p) => p.file && p.uploaded)
        .map((p) => p.uploaded as string),
      this.headers,
    );
  }

  private resolvePictures(
    pictures: IPictureForm[],
    folder: string,
  ): Observable<{ Src: string; Alt: string }[]> {
    const pending = pictures.filter((p) => p.file && !p.uploaded);

    const uploaded$ =
      pending.length === 0
        ? of(null)
        : this.productsService
            .uploadImages(
              pending.map((p) => p.file as File),
              folder,
              this.headers,
            )
            .pipe(
              map((result: any) => {
                (result.paths as string[]).forEach(
                  (path, i) => (pending[i].uploaded = path),
                );
                return null;
              }),
            );

    return uploaded$.pipe(
      map(() =>
        pictures.map((p) => ({ Src: p.uploaded || p.src, Alt: p.alt })),
      ),
    );
  }

  onCreateFilesSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (!input.files || input.files.length === 0) {
      return;
    }

    const files = Array.from(input.files);
    input.value = '';
    const validationError = this.validateSelectedFiles(
      files,
      this.createPictures.length,
    );

    if (validationError) {
      this.toastService.error(validationError);
      return;
    }

    this.createPictures.push(...this.pendingPictures(files));
  }

  removeCreatePicture(index: number): void {
    this.discardPictures(this.createPictures.splice(index, 1));
  }

  onEditFilesSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (!input.files || input.files.length === 0) {
      return;
    }

    const files = Array.from(input.files);
    input.value = '';
    const validationError = this.validateSelectedFiles(
      files,
      this.editPictures.length,
    );

    if (validationError) {
      this.toastService.error(validationError);
      return;
    }

    this.editPictures.push(...this.pendingPictures(files));
  }

  removeEditPicture(index: number): void {
    this.discardPictures(this.editPictures.splice(index, 1));
  }

  createProduct(): void {
    if (this.createForm.invalid) {
      this.createForm.markAllAsTouched();
      this.toastService.error('Please fill in all required fields.');
      return;
    }

    if (this.createSelectedSizes.length === 0) {
      this.toastService.error('Select at least one size.');
      return;
    }

    if (this.createPictures.length === 0) {
      this.toastService.error('Add at least one image.');
      return;
    }

    const dataToSend = {
      ProductName: this.createForm.get('productName')?.value,
      CategoryId: this.createForm.get('categoryId')?.value,
      BrandId: this.createForm.get('brandId')?.value,
      GenderId: this.createForm.get('genderId')?.value,
      InStock: this.createForm.get('inStock')?.value,
      Material: this.createForm.get('material')?.value,
      CountryId: this.createForm.get('countryId')?.value,
      Price: {
        Price: this.createForm.get('price')?.value,
        ActiveFrom: new Date().toISOString(),
      },
      Sizes: this.createSelectedSizes.map((id) => ({
        SizeId: id,
        Quantity: this.createSizeQuantities[id] || 0,
      })),
    };

    const folder = this.categoryFolderName(
      this.createForm.get('categoryId')?.value,
    );

    if (!folder) {
      this.toastService.error('Choose a category first.');
      return;
    }

    this.creating = true;

    this.resolvePictures(this.createPictures, folder)
      .pipe(
        switchMap((pictures) =>
          this.productsService.create(
            { ...dataToSend, Pictures: pictures },
            this.headers,
          ),
        ),
      )
      .subscribe({
        next: () => {
          this.creating = false;
          this.releasePictures(this.createPictures);
          this.createForm.reset({
            sale: false,
            inStock: false,
            countryId: null,
          });
          this.createSelectedSizes = [];
          this.createSizeQuantities = {};
          this.createPictures = [];
          this.showCreateForm = false;
          this.toastService.success('Product created successfully.');
          this.loadProducts();
        },
        error: (err: any) => {
          this.creating = false;

          this.toastService.error(
            err?.error?.errors?.[0]?.error ||
              'Could not create product. Name might already be in use.',
          );
        },
      });
  }

  startEdit(product: any): void {
    this.showCreateForm = false;
    this.editingProductId = product.id;
    this.editSelectedSizes = (product.sizeStocks || []).map((s: any) => s.id);
    this.editSizeQuantities = {};

    (product.sizeStocks || []).forEach((s: any) => {
      this.editSizeQuantities[s.id] = s.quantity || 0;
    });

    this.discardPictures(this.editPictures);
    this.editPictures = (product.picture || []).map((p: any) => ({
      src: p.src,
      alt: p.alt,
    }));

    this.editOnSale =
      product.oldPrice != null && product.oldPrice > product.newPrice;
    this.editRegularPrice = this.editOnSale
      ? product.oldPrice
      : product.newPrice;

    this.editForm.setValue({
      productId: product.id,
      productName: product.productName,
      categoryId: product.categoryId,
      brandId: product.brandId,
      genderId: product.genderId,
      sale: product.sale,
      inStock: this.editSelectedSizes.some(
        (id: number) => (this.editSizeQuantities[id] || 0) > 0,
      ),
      material: product.material || '',
      countryId: product.countryId || null,
      price: this.editRegularPrice,
    });
  }

  cancelEdit(): void {
    this.discardPictures(this.editPictures);
    this.editPictures = [];
    this.editingProductId = null;
  }

  saveEdit(): void {
    if (this.editForm.invalid) {
      this.editForm.markAllAsTouched();
      this.toastService.error('Please fill in all required fields.');
      return;
    }

    if (this.editSelectedSizes.length === 0) {
      this.toastService.error('Select at least one size.');
      return;
    }

    if (this.editPictures.length === 0) {
      this.toastService.error('Add at least one image.');
      return;
    }

    const dataToSend = {
      ProductId: this.editForm.get('productId')?.value,
      ProductName: this.editForm.get('productName')?.value,
      CategoryId: this.editForm.get('categoryId')?.value,
      BrandId: this.editForm.get('brandId')?.value,
      GenderId: this.editForm.get('genderId')?.value,
      InStock: this.editForm.get('inStock')?.value,
      Material: this.editForm.get('material')?.value,
      CountryId: this.editForm.get('countryId')?.value,
      Sizes: this.editSelectedSizes.map((id) => ({
        SizeId: id,
        Quantity: this.editSizeQuantities[id] || 0,
      })),
    };

    const folder = this.categoryFolderName(
      this.editForm.get('categoryId')?.value,
    );

    if (!folder) {
      this.toastService.error('Choose a category first.');
      return;
    }

    const newPrice = Number(this.editForm.get('price')?.value);
    const priceChanged = newPrice !== this.editRegularPrice;

    this.saving = true;

    this.resolvePictures(this.editPictures, folder)
      .pipe(
        switchMap((pictures) =>
          this.productsService.update(
            { ...dataToSend, Pictures: pictures },
            this.headers,
          ),
        ),
        switchMap(() =>
          priceChanged
            ? this.productsService.changePrice(
                dataToSend.ProductId,
                newPrice,
                this.headers,
              )
            : of(null),
        ),
      )
      .subscribe({
        next: () => {
          this.saving = false;
          this.releasePictures(this.editPictures);
          this.editPictures = [];
          this.editingProductId = null;
          this.toastService.success('Product updated successfully.');
          this.loadProducts();
        },
        error: (err: any) => {
          this.saving = false;
          this.toastService.error(
            err?.error?.errors?.[0]?.error || 'Could not update product.',
          );
        },
      });
  }

  async deleteProduct(
    id: number,
    name: string,
    isActive: boolean,
  ): Promise<void> {
    const message = isActive
      ? `Delete "${name}"? It will be hidden from the storefront unless you show inactive products.`
      : `Restore "${name}"?`;

    const confirmed = await this.confirmService.confirm(
      message,
      isActive ? 'Delete product' : 'Restore product',
      isActive ? 'Delete' : 'Restore',
      isActive,
    );

    if (!confirmed) {
      return;
    }

    this.productsService.delete(id, this.headers).subscribe({
      next: () => {
        this.toastService.success(
          isActive ? 'Product deleted.' : 'Product restored.',
        );
        this.loadProducts();
      },
      error: (err: any) => {
        this.toastService.error(
          err?.error?.errors?.[0]?.error ||
            err?.error?.message ||
            'Could not update product.',
        );
      },
    });
  }
}
