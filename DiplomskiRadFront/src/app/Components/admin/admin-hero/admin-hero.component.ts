import { Component, OnInit } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';
import { HeroSlideService } from '../../../Services/hero-slide.service';
import { IHeroSlide } from '../../../Interfaces/i-hero-slide';
import { ToastService } from '../../../Services/toast.service';
import { ConfirmService } from '../../../Services/confirm.service';
import { UploadService } from '../../../Services/upload.service';
import { withMinDuration } from '../../../Utils/loading.util';

interface IHeroForm {
  tagline: string;
  title: string;
  subtitle: string;
  buttonText: string;
  buttonLink: string;
  mediaType: 'image' | 'video';
  mediaSrc: string;
  sortOrder: number;
}

@Component({
  selector: 'app-admin-hero',
  templateUrl: './admin-hero.component.html',
  styleUrls: ['./admin-hero.component.scss'],
})
export class AdminHeroComponent implements OnInit {
  headers: any;
  slides: IHeroSlide[] = [];
  dataLoaded: boolean = false;
  loading: boolean = false;
  includeInactive: boolean = true;

  showForm: boolean = false;
  editingId: number | null = null;
  saving: boolean = false;
  uploading: boolean = false;
  form: IHeroForm = this.emptyForm();
  pendingFile: File | null = null;
  pendingPreview: string = '';
  uploadedPath: string | null = null;

  constructor(
    private heroSlideService: HeroSlideService,
    private toastService: ToastService,
    private confirmService: ConfirmService,
    private uploadService: UploadService,
  ) {}

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    this.headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    this.load();
  }

  private emptyForm(): IHeroForm {
    return {
      tagline: '',
      title: '',
      subtitle: '',
      buttonText: 'Shop now',
      buttonLink: '/shop',
      mediaType: 'image',
      mediaSrc: '',
      sortOrder: 1,
    };
  }

  load(): void {
    this.loading = true;

    withMinDuration(
      this.heroSlideService.getSlides(this.headers, this.includeInactive),
    ).subscribe({
      next: (data: IHeroSlide[]) => {
        this.slides = data;
        this.dataLoaded = true;
        this.loading = false;
      },
      error: () => {
        this.dataLoaded = true;
        this.loading = false;
        this.toastService.error('Could not load hero slides.');
      },
    });
  }

  get previewSrc(): string {
    return this.pendingPreview || this.form.mediaSrc;
  }

  private dropPending(): void {
    if (this.pendingPreview) {
      URL.revokeObjectURL(this.pendingPreview);
    }

    if (this.uploadedPath) {
      this.uploadService.discard([this.uploadedPath], this.headers);
    }

    this.pendingFile = null;
    this.pendingPreview = '';
    this.uploadedPath = null;
  }

  openCreate(): void {
    this.dropPending();
    this.editingId = null;
    this.form = this.emptyForm();
    this.form.sortOrder =
      this.slides.length > 0
        ? Math.max(...this.slides.map((s) => s.sortOrder)) + 1
        : 1;
    this.showForm = true;
  }

  startEdit(slide: IHeroSlide): void {
    this.dropPending();
    this.editingId = slide.id;

    this.form = {
      tagline: slide.tagline || '',
      title: slide.title,
      subtitle: slide.subtitle || '',
      buttonText: slide.buttonText || '',
      buttonLink: slide.buttonLink || '',
      mediaType: slide.mediaType,
      mediaSrc: slide.mediaSrc || '',
      sortOrder: slide.sortOrder,
    };

    this.showForm = true;
    setTimeout(() =>
      document
        .getElementById('hero-form')
        ?.scrollIntoView({ behavior: 'smooth', block: 'start' }),
    );
  }

  cancelForm(): void {
    this.dropPending();
    this.showForm = false;
    this.editingId = null;
  }

  get acceptedFiles(): string {
    return this.form.mediaType === 'video'
      ? '.mp4,.webm'
      : '.jpg,.jpeg,.png,.webp';
  }

  setMediaType(type: 'image' | 'video'): void {
    if (this.form.mediaType !== type) {
      this.dropPending();
      this.form.mediaType = type;
      this.form.mediaSrc = '';
    }
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files && input.files[0];
    input.value = '';

    if (!file) {
      return;
    }

    const isVideo = this.form.mediaType === 'video';
    const extension = file.name
      .substring(file.name.lastIndexOf('.'))
      .toLowerCase();

    if (!this.acceptedFiles.split(',').includes(extension)) {
      this.toastService.error(
        isVideo
          ? 'Please choose an MP4 or WebM video.'
          : 'Please choose a JPG, PNG or WebP image.',
      );

      return;
    }

    if (file.size > (isVideo ? 50 : 8) * 1024 * 1024) {
      this.toastService.error(
        'The file must be ' + (isVideo ? 50 : 8) + ' MB or smaller.',
      );
      return;
    }

    this.dropPending();
    this.pendingFile = file;
    this.pendingPreview = URL.createObjectURL(file);
  }

  clearMedia(): void {
    this.dropPending();
    this.form.mediaSrc = '';
  }

  save(): void {
    if (!this.form.title.trim()) {
      this.toastService.error('Title is required.');
      return;
    }

    if (this.form.buttonText.trim() && !this.form.buttonLink.trim()) {
      this.toastService.error('A button needs a link as well as a label.');
      return;
    }

    if (this.pendingFile && !this.uploadedPath) {
      this.saving = true;
      this.uploading = true;

      this.heroSlideService.upload(this.pendingFile, this.headers).subscribe({
        next: (result) => {
          this.uploadedPath = result.path;
          this.uploading = false;
          this.send();
        },
        error: (err: any) => {
          this.saving = false;
          this.uploading = false;
          this.toastService.error(
            err?.error?.errors?.[0]?.error || 'Could not upload the file.',
          );
        },
      });

      return;
    }

    this.saving = true;
    this.send();
  }

  private send(): void {
    const payload = {
      Tagline: this.form.tagline,
      Title: this.form.title,
      Subtitle: this.form.subtitle,
      ButtonText: this.form.buttonText,
      ButtonLink: this.form.buttonLink,
      MediaType: this.form.mediaType,
      MediaSrc:
        this.uploadedPath || (this.pendingFile ? '' : this.form.mediaSrc),
      SortOrder: Number(this.form.sortOrder) || 0,
    };

    const request =
      this.editingId == null
        ? this.heroSlideService.create(payload, this.headers)
        : this.heroSlideService.update(this.editingId, payload, this.headers);

    request.subscribe({
      next: () => {
        this.saving = false;
        this.toastService.success(
          this.editingId == null ? 'Hero slide created.' : 'Hero slide saved.',
        );
        this.uploadedPath = null;
        this.cancelForm();
        this.load();
      },
      error: (err: any) => {
        this.saving = false;
        this.toastService.error(
          err?.error?.errors?.[0]?.error || 'Could not save the hero slide.',
        );
      },
    });
  }

  async toggleActive(slide: IHeroSlide): Promise<void> {
    const confirmed = await this.confirmService.confirm(
      slide.isActive
        ? `"${slide.title}" will no longer be shown on the home page.`
        : `"${slide.title}" will be shown on the home page again.`,
      slide.isActive ? 'Deactivate slide' : 'Reactivate slide',
      slide.isActive ? 'Deactivate' : 'Reactivate',
      slide.isActive,
    );

    if (!confirmed) {
      return;
    }

    this.heroSlideService.delete(slide.id, this.headers).subscribe({
      next: () => {
        this.toastService.success(
          slide.isActive ? 'Slide deactivated.' : 'Slide reactivated.',
        );
        this.load();
      },
      error: () => this.toastService.error('Could not update the slide.'),
    });
  }
}
