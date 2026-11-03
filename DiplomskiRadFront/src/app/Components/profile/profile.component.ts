import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpHeaders } from '@angular/common/http';
import { JwtHelperService } from '@auth0/angular-jwt';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { UserService } from '../../Services/user.service';
import { ReceiptService } from '../../Services/receipt.service';
import { ToastService } from '../../Services/toast.service';
import { ProductRatingService } from '../../Services/product-rating.service';
import { UploadService } from '../../Services/upload.service';
import { Observable, of, throwError, catchError, switchMap } from 'rxjs';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss'],
})
export class ProfileComponent implements OnInit {
  headers: any;
  userId: number = 0;
  isAdmin: boolean = false;

  activeTab: 'general' | 'orders' = 'general';

  user: any;
  userDataLoaded: boolean = false;
  editForm!: FormGroup;
  saving: boolean = false;
  pendingAvatarFile: File | null = null;
  pendingAvatarPreview: string = '';
  avatarRemoved: boolean = false;

  orders: any[] = [];
  ordersLoaded: boolean = false;
  expandedOrderIds: Set<number> = new Set<number>();
  yearFilter: string = 'all';
  availableYears: number[] = [];

  ratingItemKey: string | null = null;
  ratingItem: any = null;
  ratingValue: number = 0;
  ratingComment: string = '';
  submittingRating: boolean = false;

  constructor(
    private jwtHelper: JwtHelperService,
    private router: Router,
    private route: ActivatedRoute,
    private userService: UserService,
    private receiptService: ReceiptService,
    private toastService: ToastService,
    private productRatingService: ProductRatingService,
    private uploadService: UploadService,
  ) {}

  ngOnInit(): void {
    let token: string | null = localStorage.getItem('token');

    if (!token) {
      this.router.navigate(['/home']);
      return;
    }

    this.headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    let decodedToken = this.jwtHelper.decodeToken(token);
    this.userId = decodedToken.UserId;
    this.isAdmin = Number(decodedToken.Role) === 1;

    const requestedTab = this.route.snapshot.data['tab'];

    if (requestedTab === 'orders' && !this.isAdmin) {
      this.activeTab = 'orders';
    }

    this.editForm = new FormGroup<any>({
      firstName: new FormControl('', [
        Validators.required,
        Validators.minLength(3),
      ]),
      lastName: new FormControl('', [
        Validators.required,
        Validators.minLength(3),
      ]),
      address: new FormControl(''),
      city: new FormControl(''),
      postalCode: new FormControl(''),
      country: new FormControl(''),
    });

    this.loadUser();

    if (!this.isAdmin) {
      this.loadOrders();
    }
  }

  get initials(): string {
    return [this.user?.firstName, this.user?.lastName]
      .filter((x) => !!x)
      .map((x) => x[0])
      .slice(0, 2)
      .join('')
      .toUpperCase();
  }

  get avatarPreview(): string {
    return (
      this.pendingAvatarPreview ||
      (this.avatarRemoved ? '' : this.user?.avatarSrc || '')
    );
  }

  get avatarModified(): boolean {
    return !!this.pendingAvatarFile || this.avatarRemoved;
  }

  onAvatarSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files && input.files[0];
    input.value = '';

    if (!file) {
      return;
    }

    const extension = file.name
      .substring(file.name.lastIndexOf('.'))
      .toLowerCase();

    if (!['.jpg', '.jpeg', '.png', '.webp'].includes(extension)) {
      this.toastService.error('Please choose a JPG, PNG or WebP image.');
      return;
    }

    if (file.size > 2 * 1024 * 1024) {
      this.toastService.error('The picture must be 2 MB or smaller.');
      return;
    }

    this.resetAvatarChange();
    this.pendingAvatarFile = file;
    this.pendingAvatarPreview = URL.createObjectURL(file);
  }

  removeAvatar(): void {
    this.resetAvatarChange();
    this.avatarRemoved = !!this.user?.avatarSrc;
  }

  resetAvatarChange(): void {
    if (this.pendingAvatarPreview) {
      URL.revokeObjectURL(this.pendingAvatarPreview);
    }

    this.pendingAvatarFile = null;
    this.pendingAvatarPreview = '';
    this.avatarRemoved = false;
  }

  private saveAvatarChange(): Observable<any> {
    if (this.pendingAvatarFile) {
      return this.userService
        .uploadAvatar(this.pendingAvatarFile, this.headers)
        .pipe(
          switchMap((result: any) =>
            this.userService.updateAvatar(result.path, this.headers).pipe(
              switchMap(() => {
                this.user.avatarSrc = result.path;
                return of(null);
              }),
              catchError((err) => {
                this.uploadService.discard([result.path], this.headers);
                return throwError(() => err);
              }),
            ),
          ),
        );
    }

    if (this.avatarRemoved) {
      return this.userService.updateAvatar('', this.headers).pipe(
        switchMap(() => {
          this.user.avatarSrc = null;
          return of(null);
        }),
      );
    }

    return of(null);
  }

  selectTab(tab: 'general' | 'orders'): void {
    this.activeTab = tab;
  }

  loadUser(): void {
    this.userService.get(this.userId, this.headers).subscribe({
      next: (data: any) => {
        this.user = data;
        this.userDataLoaded = true;

        this.editForm.setValue({
          firstName: data.firstName || '',
          lastName: data.lastName || '',
          address: data.info?.address || '',
          city: data.info?.city || '',
          postalCode: data.info?.postalCode || '',
          country: data.info?.country || '',
        });
      },
      error: (err: any) => {
        if (err.status === 403) {
          this.toastService.error('You do not have access to that account.');
          this.router.navigate(['/home']);
          return;
        }

        this.userDataLoaded = true;
        this.toastService.error('Could not load your account info.');
      },
    });
  }

  saveChanges(): void {
    if (this.editForm.invalid) {
      this.editForm.markAllAsTouched();
      return;
    }

    const dataToSend = {
      UserId: this.userId,
      FirstName: this.editForm.get('firstName')?.value,
      LastName: this.editForm.get('lastName')?.value,
      Address: this.editForm.get('address')?.value,
      City: this.editForm.get('city')?.value,
      PostalCode: this.editForm.get('postalCode')?.value,
      Country: this.editForm.get('country')?.value,
    };

    this.saving = true;

    this.saveAvatarChange()
      .pipe(switchMap(() => this.userService.update(dataToSend, this.headers)))
      .subscribe({
        next: () => {
          this.saving = false;
          this.resetAvatarChange();
          this.userService.avatarChanged.next(this.user.avatarSrc);
          this.toastService.success('Your info was updated.');
          this.loadUser();
        },
        error: (err: any) => {
          this.saving = false;
          this.toastService.errorFrom(err, 'Could not update your info.');
        },
      });
  }

  loadOrders(): void {
    this.receiptService.search(this.userId, false, this.headers).subscribe({
      next: (data: any) => {
        this.orders = data.data;
        this.ordersLoaded = true;

        this.availableYears = Array.from(
          new Set(
            this.orders.map((o: any) => new Date(o.orderedAt).getFullYear()),
          ),
        ).sort((a, b) => b - a);
      },
      error: () => {
        this.ordersLoaded = true;
        this.orders = [];
        this.toastService.error('Could not load your orders.');
      },
    });
  }

  get filteredOrders(): any[] {
    if (this.yearFilter === 'all') {
      return this.orders;
    }

    return this.orders.filter(
      (o: any) =>
        new Date(o.orderedAt).getFullYear() === Number(this.yearFilter),
    );
  }

  toggleExpand(receiptId: number): void {
    if (this.expandedOrderIds.has(receiptId)) {
      this.expandedOrderIds.delete(receiptId);
    } else {
      this.expandedOrderIds.add(receiptId);
    }
  }

  starClass(star: number, rating: number): string {
    const r = rating || 0;

    if (star <= Math.floor(r)) {
      return 'fas fa-star';
    }

    if (star - r === 0.5) {
      return 'fas fa-star-half-alt';
    }

    return 'far fa-star';
  }

  setRatingHalf(star: number): void {
    this.ratingValue = star - 0.5;
  }

  setRatingFull(star: number): void {
    this.ratingValue = star;
  }

  startRating(receiptId: number, item: any): void {
    this.ratingItemKey = receiptId + '_' + item.productId;
    this.ratingItem = item;
    this.ratingValue = item.myRating || 0;
    this.ratingComment = item.myComment || '';
  }

  cancelRating(): void {
    this.ratingItemKey = null;
    this.ratingItem = null;
    this.ratingValue = 0;
    this.ratingComment = '';
  }

  submitRating(): void {
    if (this.ratingValue < 1 || !this.ratingItem) {
      return;
    }

    this.submittingRating = true;

    this.productRatingService
      .rate(
        this.ratingItem.productId,
        this.ratingValue,
        this.ratingComment,
        this.headers,
      )
      .subscribe({
        next: () => {
          this.submittingRating = false;
          this.toastService.success('Thanks for your review!');
          this.cancelRating();
          this.loadOrders();
        },
        error: (err: any) => {
          this.submittingRating = false;
          this.toastService.errorFrom(err, 'Could not save your rating.');
        },
      });
  }
}
