import { NgModule } from '@angular/core';
import { RouteReuseStrategy, RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './Components/home/home.component';
import { ShopComponent } from './Components/shop/shop.component';
import { SingleProductComponent } from './Components/single-product/single-product.component';
import { brandResolver } from './Resolver/brand.resolver';
import { categoryResolver } from './Resolver/category.resolver';
import { genderResolver } from './Resolver/gender.resolver';
import { sizeResolver } from './Resolver/size.resolver';
import { CartComponent } from './Components/cart/cart.component';
import { ContactComponent } from './Components/contact/contact.component';
import { CustomRouteReuseStrategy } from './RouteStrategy/custom-route-reuse-strategy';
import { CheckoutComponent } from './Components/checkout/checkout.component';
import { AboutUsComponent } from './Components/about-us/about-us.component';
import { LoginComponent } from './Components/login/login.component';
import { RegisterComponent } from './Components/register/register.component';
import { ForgotPasswordComponent } from './Components/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './Components/reset-password/reset-password.component';
import { ProfileComponent } from './Components/profile/profile.component';
import { AdminUsersComponent } from './Components/admin/admin-users/admin-users.component';
import { AdminProductsComponent } from './Components/admin/admin-products/admin-products.component';
import { AdminOrdersComponent } from './Components/admin/admin-orders/admin-orders.component';
import { AdminCatalogComponent } from './Components/admin/admin-catalog/admin-catalog.component';
import { AdminStatsComponent } from './Components/admin/admin-stats/admin-stats.component';
import { AdminMessagesComponent } from './Components/admin/admin-messages/admin-messages.component';
import { AdminNewsletterComponent } from './Components/admin/admin-newsletter/admin-newsletter.component';
import { AdminCouponsComponent } from './Components/admin/admin-coupons/admin-coupons.component';
import { AdminPaymentMethodsComponent } from './Components/admin/admin-payment-methods/admin-payment-methods.component';
import { AdminHeroComponent } from './Components/admin/admin-hero/admin-hero.component';
import { AdminAuditLogsComponent } from './Components/admin/admin-audit-logs/admin-audit-logs.component';
import { AdminPromotionsComponent } from './Components/admin/admin-promotions/admin-promotions.component';
import { OrderConfirmationComponent } from './Components/order-confirmation/order-confirmation.component';
import { authGuard } from './Guards/auth.guard';
import { adminGuard } from './Guards/admin.guard';
import { guestGuard } from './Guards/guest.guard';
import { customerGuard } from './Guards/customer.guard';

const routes: Routes = [
  {
    path: '',
    redirectTo: '/home',
    pathMatch: 'full',
  },
  {
    path: 'home',
    component: HomeComponent,
    resolve: {
      brand: brandResolver,
    },
  },
  {
    path: 'shop',
    component: ShopComponent,
    resolve: {
      category: categoryResolver,
      brand: brandResolver,
      gender: genderResolver,
      size: sizeResolver,
    },
  },
  {
    path: 'cart',
    component: CartComponent,
    resolve: {},
    canActivate: [customerGuard],
  },
  {
    path: 'checkout',
    component: CheckoutComponent,
    resolve: {},
    canActivate: [authGuard, customerGuard],
  },
  {
    path: 'about',
    component: AboutUsComponent,
  },
  {
    path: 'product/:id',
    component: SingleProductComponent,
    resolve: {
      brand: brandResolver,
      category: categoryResolver,
      gender: genderResolver,
    },
  },
  {
    path: 'contact',
    component: ContactComponent,
  },
  {
    path: 'login',
    component: LoginComponent,
    canActivate: [guestGuard],
  },
  {
    path: 'register',
    component: RegisterComponent,
    canActivate: [guestGuard],
  },
  {
    path: 'forgot-password',
    component: ForgotPasswordComponent,
    canActivate: [guestGuard],
  },
  {
    path: 'reset-password',
    component: ResetPasswordComponent,
    canActivate: [guestGuard],
  },
  {
    path: 'profile',
    component: ProfileComponent,
    canActivate: [authGuard],
  },
  {
    path: 'orders',
    component: ProfileComponent,
    data: { tab: 'orders' },
    canActivate: [authGuard, customerGuard],
  },
  {
    path: 'order-confirmation/:id',
    component: OrderConfirmationComponent,
    canActivate: [authGuard, customerGuard],
  },
  {
    path: 'admin/users',
    component: AdminUsersComponent,
    canActivate: [adminGuard],
  },
  {
    path: 'admin/products',
    component: AdminProductsComponent,
    canActivate: [adminGuard],
  },
  {
    path: 'admin/orders',
    component: AdminOrdersComponent,
    canActivate: [adminGuard],
  },
  {
    path: 'admin/catalog',
    component: AdminCatalogComponent,
    canActivate: [adminGuard],
  },
  {
    path: 'admin/stats',
    component: AdminStatsComponent,
    canActivate: [adminGuard],
  },
  {
    path: 'admin/messages',
    component: AdminMessagesComponent,
    canActivate: [adminGuard],
  },
  {
    path: 'admin/newsletter',
    component: AdminNewsletterComponent,
    canActivate: [adminGuard],
  },
  {
    path: 'admin/coupons',
    component: AdminCouponsComponent,
    canActivate: [adminGuard],
  },
  {
    path: 'admin/promotions',
    component: AdminPromotionsComponent,
    canActivate: [adminGuard],
  },
  {
    path: 'admin/audit-logs',
    component: AdminAuditLogsComponent,
    canActivate: [adminGuard],
  },
  {
    path: 'admin/hero',
    component: AdminHeroComponent,
    canActivate: [adminGuard],
  },
  {
    path: 'admin/payment-methods',
    component: AdminPaymentMethodsComponent,
    canActivate: [adminGuard],
  },
  {
    path: '**',
    component: HomeComponent,
    resolve: {
      brand: brandResolver,
    },
  },
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes),
    RouterModule.forChild([
      {
        path: 'product/:id',
        component: SingleProductComponent,
        data: {
          reuse: true,
        },
      },
    ]),
  ],
  providers: [
    {
      provide: RouteReuseStrategy,
      useClass: CustomRouteReuseStrategy,
    },
  ],
  exports: [RouterModule],
})
export class AppRoutingModule {}
