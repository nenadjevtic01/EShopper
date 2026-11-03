import { Component, HostListener, OnInit } from '@angular/core';
import { INav } from '../../Interfaces/i-nav';
import { NavigationService } from '../../Services/navigation.service';
import { CategoryService } from '../../Services/category.service';
import { ICategory } from '../../Interfaces/i-category';
import { CartService } from '../../Services/cart.service';
import { UserService } from '../../Services/user.service';
import { GuestCartService } from '../../Services/guest-cart.service';
import { NavigationEnd, Router } from '@angular/router';
import { HttpHeaders } from '@angular/common/http';
import { JwtHelperService } from '@auth0/angular-jwt';
import { filter } from 'rxjs';

@Component({
  selector: 'app-navigation',
  templateUrl: './navigation.component.html',
  styleUrls: ['./navigation.component.scss'],
})
export class NavigationComponent implements OnInit {
  navigation: INav[] = [];
  categories: ICategory[] = [];
  cartCount: number = 0;
  currentUserName: string = '';
  avatarSrc: string | null = null;
  menuOpen: boolean = false;
  catOpen: boolean = false;
  userOpen: boolean = false;
  scrolled: boolean = false;

  constructor(
    private jwtHelper: JwtHelperService,
    private navigationService: NavigationService,
    private categoryService: CategoryService,
    private cartService: CartService,
    private router: Router,
    private guestCartService: GuestCartService,
    private userService: UserService,
  ) {}

  private loggedInAsUserId: number | null = null;

  ngOnInit(): void {
    this.cartService.getCartCountObservable().subscribe((count) => {
      this.cartCount = count;
    });

    this.userService.avatarChanged.subscribe((src) => {
      this.avatarSrc = src;
    });

    this.refreshUserContext();

    this.router.events
      .pipe(filter((e) => e instanceof NavigationEnd))
      .subscribe(() => {
        this.refreshUserContext();
        this.closeMenus();
      });

    this.navigationService.getNav().subscribe({
      next: (data) => {
        this.navigation = data;
      },
      error: () => {},
    });

    this.categoryService.getCategory().subscribe({
      next: (data) => {
        this.categories = data;
      },
      error: () => {},
    });
  }

  private refreshUserContext(): void {
    let token: string | null = localStorage.getItem('token');

    if (!token) {
      this.loggedInAsUserId = null;
      this.currentUserName = '';
      this.avatarSrc = null;
      this.guestCartService.refreshCount();
      return;
    }

    const decodedToken = this.jwtHelper.decodeToken(token);
    let userId = Number(decodedToken.UserId);
    this.currentUserName = [decodedToken.FirstName, decodedToken.LastName]
      .filter((x) => !!x)
      .join(' ');

    if (this.loggedInAsUserId === userId) {
      return;
    }

    this.loggedInAsUserId = userId;

    const userHeaders = new HttpHeaders().set(
      'Authorization',
      `Bearer ${token}`,
    );

    this.userService.get(userId, userHeaders).subscribe({
      next: (data: any) => {
        this.avatarSrc = data.avatarSrc || null;
      },
      error: (err: any) => {
        if (err.status === 403) {
          this.router.navigate(['/home']);
        }
      },
    });

    if (Number(decodedToken.Role) === 1) {
      this.cartService.updateCartCount(0);
      return;
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    this.cartService.get(userId, headers).subscribe({
      error: () => {},
    });
  }

  closeMenus(): void {
    this.menuOpen = false;
    this.catOpen = false;
    this.userOpen = false;
  }

  toggleMenu(): void {
    this.menuOpen = !this.menuOpen;
    this.catOpen = false;
    this.userOpen = false;
  }

  toggleCategories(event: Event): void {
    event.stopPropagation();
    this.catOpen = !this.catOpen;
    this.userOpen = false;
  }

  toggleUser(event: Event): void {
    event.stopPropagation();
    this.userOpen = !this.userOpen;
    this.catOpen = false;
  }

  get initials(): string {
    return this.currentUserName
      .split(' ')
      .filter((x) => !!x)
      .map((x) => x[0])
      .slice(0, 2)
      .join('')
      .toUpperCase();
  }

  @HostListener('document:click')
  onDocumentClick(): void {
    this.catOpen = false;
    this.userOpen = false;
  }

  @HostListener('window:scroll')
  onScroll(): void {
    this.scrolled = window.scrollY > 8;
  }

  isLoggedIn(): boolean {
    let token: string | null;
    token = localStorage.getItem('token');
    return token != null;
  }

  isAdmin(): boolean {
    let token: string | null = localStorage.getItem('token');

    if (!token) {
      return false;
    }

    const decodedToken = this.jwtHelper.decodeToken(token);
    return Number(decodedToken.Role) === 1;
  }

  logout(): any {
    this.closeMenus();
    localStorage.removeItem('token');
    this.refreshUserContext();
    this.router.navigate(['/home']);
  }
}
