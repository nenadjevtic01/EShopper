import { Component, OnInit } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { CategoryService } from '../../../Services/category.service';
import { BrandService } from '../../../Services/brand.service';
import { ToastService } from '../../../Services/toast.service';
import { ConfirmService } from '../../../Services/confirm.service';
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
  selector: 'app-admin-catalog',
  templateUrl: './admin-catalog.component.html',
  styleUrls: ['./admin-catalog.component.scss'],
})
export class AdminCatalogComponent implements OnInit {
  headers: any;

  categories: any[] = [];
  brands: any[] = [];
  loading: boolean = false;
  dataLoaded: boolean = false;
  includeInactive: boolean = false;

  readonly allowedImageTypes: string[] = [
    'image/jpeg',
    'image/jpg',
    'image/png',
  ];

  showCreateCategoryForm: boolean = false;
  createCategoryForm!: FormGroup;
  createCategoryPicture: IPictureForm | null = null;
  editingCategoryId: number | null = null;
  editCategoryForm!: FormGroup;
  editCategoryPicture: IPictureForm | null = null;

  showCreateBrandForm: boolean = false;
  createBrandForm!: FormGroup;
  createBrandPicture: IPictureForm | null = null;
  editingBrandId: number | null = null;
  editBrandForm!: FormGroup;
  editBrandPicture: IPictureForm | null = null;

  saving: boolean = false;

  constructor(
    private categoryService: CategoryService,
    private brandService: BrandService,
    private toastService: ToastService,
    private confirmService: ConfirmService,
    private uploadService: UploadService,
  ) {}

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    this.headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    this.createCategoryForm = new FormGroup<any>({
      name: new FormControl('', [Validators.required]),
    });

    this.editCategoryForm = new FormGroup<any>({
      id: new FormControl(0),
      name: new FormControl('', [Validators.required]),
    });

    this.createBrandForm = new FormGroup<any>({
      name: new FormControl('', [Validators.required]),
    });

    this.editBrandForm = new FormGroup<any>({
      id: new FormControl(0),
      name: new FormControl('', [Validators.required]),
    });

    this.loadAll();
  }

  loadAll(): void {
    this.loading = true;

    withMinDuration(
      this.categoryService.getCategory(this.includeInactive, this.headers),
    ).subscribe({
      next: (data) => {
        this.categories = data;
        this.loading = false;
        this.dataLoaded = true;
      },
      error: () => {
        this.loading = false;
        this.dataLoaded = true;
        this.toastService.error('Could not load categories.');
      },
    });

    this.brandService.getBrands(this.includeInactive, this.headers).subscribe({
      next: (data) => {
        this.brands = data;
      },
      error: () => {
        this.toastService.error('Could not load brands.');
      },
    });
  }

  private pendingPicture(file: File, alt: string): IPictureForm {
    return { src: URL.createObjectURL(file), alt, file };
  }

  private releasePicture(picture: IPictureForm | null): void {
    if (picture?.file) {
      URL.revokeObjectURL(picture.src);
    }
  }

  private discardPicture(picture: IPictureForm | null): void {
    if (!picture?.file) {
      return;
    }

    this.releasePicture(picture);

    if (picture.uploaded) {
      this.uploadService.discard([picture.uploaded], this.headers);
    }
  }

  private resolveSrc(
    picture: IPictureForm,
    upload: (file: File) => Observable<any>,
  ): Observable<string> {
    if (!picture.file) {
      return of(picture.src);
    }

    if (picture.uploaded) {
      return of(picture.uploaded);
    }

    return upload(picture.file).pipe(
      map((result: any) => {
        picture.uploaded = result.path;
        return result.path as string;
      }),
    );
  }

  private validateSelectedFile(file: File): string | null {
    if (!this.allowedImageTypes.includes(file.type)) {
      return `"${file.name}" is not a supported image type. Only JPG and PNG are allowed.`;
    }

    return null;
  }

  toggleCreateCategoryForm(): void {
    this.showCreateCategoryForm = !this.showCreateCategoryForm;
    this.discardPicture(this.createCategoryPicture);
    this.createCategoryPicture = null;

    if (this.showCreateCategoryForm) {
      this.editingCategoryId = null;
      this.createCategoryForm.reset();
    }
  }

  onCreateCategoryFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (!input.files || input.files.length === 0) {
      return;
    }

    const file = input.files[0];
    const validationError = this.validateSelectedFile(file);
    input.value = '';

    if (validationError) {
      this.toastService.error(validationError);
      return;
    }

    const name = this.createCategoryForm.get('name')?.value || 'Category';
    this.createCategoryPicture = this.pendingPicture(file, `${name} image`);
  }

  removeCreateCategoryPicture(): void {
    this.discardPicture(this.createCategoryPicture);
    this.createCategoryPicture = null;
  }

  createCategory(): void {
    if (this.createCategoryForm.invalid) {
      this.createCategoryForm.markAllAsTouched();
      return;
    }

    const picture = this.createCategoryPicture;

    if (!picture) {
      this.toastService.error('Upload an image for the category.');
      return;
    }

    this.saving = true;

    this.resolveSrc(picture, (file) =>
      this.categoryService.uploadImage(
        file,
        this.createCategoryForm.get('name')?.value,
        this.headers,
      ),
    )
      .pipe(
        switchMap((src) =>
          this.categoryService.create(
            {
              Name: this.createCategoryForm.get('name')?.value,
              Picture: { Src: src, Alt: picture.alt },
            },
            this.headers,
          ),
        ),
      )
      .subscribe({
        next: () => {
          this.saving = false;
          this.releasePicture(picture);
          this.createCategoryPicture = null;
          this.showCreateCategoryForm = false;
          this.toastService.success('Category created.');
          this.loadAll();
        },
        error: (err: any) => {
          this.saving = false;
          this.toastService.error(
            err?.error?.errors?.[0]?.error || 'Could not create category.',
          );
        },
      });
  }

  startEditCategory(category: any): void {
    this.showCreateCategoryForm = false;
    this.discardPicture(this.editCategoryPicture);
    this.editingCategoryId = category.id;
    this.editCategoryPicture = { src: category.img.src, alt: category.img.alt };

    this.editCategoryForm.setValue({
      id: category.id,
      name: category.name,
    });
  }

  cancelEditCategory(): void {
    this.discardPicture(this.editCategoryPicture);
    this.editCategoryPicture = null;
    this.editingCategoryId = null;
  }

  onEditCategoryFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (!input.files || input.files.length === 0) {
      return;
    }

    const file = input.files[0];
    const validationError = this.validateSelectedFile(file);
    input.value = '';

    if (validationError) {
      this.toastService.error(validationError);
      return;
    }

    const name = this.editCategoryForm.get('name')?.value || 'Category';
    this.editCategoryPicture = this.pendingPicture(file, `${name} image`);
  }

  removeEditCategoryPicture(): void {
    this.discardPicture(this.editCategoryPicture);
    this.editCategoryPicture = null;
  }

  saveEditCategory(): void {
    if (this.editCategoryForm.invalid) {
      this.editCategoryForm.markAllAsTouched();
      return;
    }

    const picture = this.editCategoryPicture;

    if (!picture) {
      this.toastService.error('Upload an image for the category.');
      return;
    }

    this.saving = true;

    this.resolveSrc(picture, (file) =>
      this.categoryService.uploadImage(
        file,
        this.editCategoryForm.get('name')?.value,
        this.headers,
        this.editCategoryForm.get('id')?.value,
      ),
    )
      .pipe(
        switchMap((src) =>
          this.categoryService.update(
            {
              Id: this.editCategoryForm.get('id')?.value,
              Name: this.editCategoryForm.get('name')?.value,
              Picture: { Src: src, Alt: picture.alt },
            },
            this.headers,
          ),
        ),
      )
      .subscribe({
        next: () => {
          this.saving = false;
          this.releasePicture(picture);
          this.editCategoryPicture = null;
          this.editingCategoryId = null;
          this.toastService.success('Category updated.');
          this.loadAll();
        },
        error: (err: any) => {
          this.saving = false;
          this.toastService.error(
            err?.error?.errors?.[0]?.error || 'Could not update category.',
          );
        },
      });
  }

  async deleteCategory(
    id: number,
    name: string,
    isActive: boolean,
  ): Promise<void> {
    const message = isActive
      ? `Delete category "${name}"? This is blocked if any product still uses it.`
      : `Restore category "${name}"?`;

    const confirmed = await this.confirmService.confirm(
      message,
      isActive ? 'Delete category' : 'Restore category',
      isActive ? 'Delete' : 'Restore',
      isActive,
    );

    if (!confirmed) {
      return;
    }

    this.categoryService.delete(id, this.headers).subscribe({
      next: () => {
        this.toastService.success(
          isActive ? 'Category deleted.' : 'Category restored.',
        );
        this.loadAll();
      },
      error: (err: any) => {
        this.toastService.error(
          err?.error?.errors?.[0]?.error ||
            err?.error?.message ||
            'Could not update category.',
        );
      },
    });
  }

  toggleCreateBrandForm(): void {
    this.showCreateBrandForm = !this.showCreateBrandForm;
    this.discardPicture(this.createBrandPicture);
    this.createBrandPicture = null;

    if (this.showCreateBrandForm) {
      this.editingBrandId = null;
      this.createBrandForm.reset();
    }
  }

  onCreateBrandFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (!input.files || input.files.length === 0) {
      return;
    }

    const file = input.files[0];
    const validationError = this.validateSelectedFile(file);
    input.value = '';

    if (validationError) {
      this.toastService.error(validationError);
      return;
    }

    const name = this.createBrandForm.get('name')?.value || 'Brand';
    this.createBrandPicture = this.pendingPicture(file, `${name} logo`);
  }

  removeCreateBrandPicture(): void {
    this.discardPicture(this.createBrandPicture);
    this.createBrandPicture = null;
  }

  createBrand(): void {
    if (this.createBrandForm.invalid) {
      this.createBrandForm.markAllAsTouched();
      return;
    }

    const picture = this.createBrandPicture;

    if (!picture) {
      this.toastService.error('Upload an image for the brand.');
      return;
    }

    this.saving = true;

    this.resolveSrc(picture, (file) =>
      this.brandService.uploadImage(file, this.headers),
    )
      .pipe(
        switchMap((src) =>
          this.brandService.create(
            {
              Name: this.createBrandForm.get('name')?.value,
              Picture: { Src: src, Alt: picture.alt },
            },
            this.headers,
          ),
        ),
      )
      .subscribe({
        next: () => {
          this.saving = false;
          this.releasePicture(picture);
          this.createBrandPicture = null;
          this.showCreateBrandForm = false;
          this.toastService.success('Brand created.');
          this.loadAll();
        },
        error: (err: any) => {
          this.saving = false;
          this.toastService.error(
            err?.error?.errors?.[0]?.error || 'Could not create brand.',
          );
        },
      });
  }

  startEditBrand(brand: any): void {
    this.showCreateBrandForm = false;
    this.discardPicture(this.editBrandPicture);
    this.editingBrandId = brand.id;
    this.editBrandPicture = { src: brand.img.src, alt: brand.img.alt };

    this.editBrandForm.setValue({
      id: brand.id,
      name: brand.name,
    });
  }

  cancelEditBrand(): void {
    this.discardPicture(this.editBrandPicture);
    this.editBrandPicture = null;
    this.editingBrandId = null;
  }

  onEditBrandFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (!input.files || input.files.length === 0) {
      return;
    }

    const file = input.files[0];
    const validationError = this.validateSelectedFile(file);
    input.value = '';

    if (validationError) {
      this.toastService.error(validationError);
      return;
    }

    const name = this.editBrandForm.get('name')?.value || 'Brand';
    this.editBrandPicture = this.pendingPicture(file, `${name} logo`);
  }

  removeEditBrandPicture(): void {
    this.discardPicture(this.editBrandPicture);
    this.editBrandPicture = null;
  }

  saveEditBrand(): void {
    if (this.editBrandForm.invalid) {
      this.editBrandForm.markAllAsTouched();
      return;
    }

    const picture = this.editBrandPicture;

    if (!picture) {
      this.toastService.error('Upload an image for the brand.');
      return;
    }

    this.saving = true;

    this.resolveSrc(picture, (file) =>
      this.brandService.uploadImage(file, this.headers),
    )
      .pipe(
        switchMap((src) =>
          this.brandService.update(
            {
              Id: this.editBrandForm.get('id')?.value,
              Name: this.editBrandForm.get('name')?.value,
              Picture: { Src: src, Alt: picture.alt },
            },
            this.headers,
          ),
        ),
      )
      .subscribe({
        next: () => {
          this.saving = false;
          this.releasePicture(picture);
          this.editBrandPicture = null;
          this.editingBrandId = null;
          this.toastService.success('Brand updated.');
          this.loadAll();
        },
        error: (err: any) => {
          this.saving = false;
          this.toastService.error(
            err?.error?.errors?.[0]?.error || 'Could not update brand.',
          );
        },
      });
  }

  async deleteBrand(
    id: number,
    name: string,
    isActive: boolean,
  ): Promise<void> {
    const message = isActive
      ? `Delete brand "${name}"? This is blocked if any product still uses it.`
      : `Restore brand "${name}"?`;

    const confirmed = await this.confirmService.confirm(
      message,
      isActive ? 'Delete brand' : 'Restore brand',
      isActive ? 'Delete' : 'Restore',
      isActive,
    );

    if (!confirmed) {
      return;
    }

    this.brandService.delete(id, this.headers).subscribe({
      next: () => {
        this.toastService.success(
          isActive ? 'Brand deleted.' : 'Brand restored.',
        );
        this.loadAll();
      },
      error: (err: any) => {
        this.toastService.error(
          err?.error?.errors?.[0]?.error ||
            err?.error?.message ||
            'Could not update brand.',
        );
      },
    });
  }
}
