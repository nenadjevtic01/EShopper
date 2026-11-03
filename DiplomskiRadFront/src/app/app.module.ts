import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavigationComponent } from './Components/navigation/navigation.component';
import { FooterComponent } from './Components/footer/footer.component';
import { HomeComponent } from './Components/home/home.component';
import { ShopComponent } from './Components/shop/shop.component';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SingleProductComponent } from './Components/single-product/single-product.component';
import { CartComponent } from './Components/cart/cart.component';
import { ContactComponent } from './Components/contact/contact.component';
import { CheckoutComponent } from './Components/checkout/checkout.component';
import { AboutUsComponent } from './Components/about-us/about-us.component';
import { LoginComponent } from './Components/login/login.component';
import { RegisterComponent } from './Components/register/register.component';
import { ProfileComponent } from './Components/profile/profile.component';
import { AdminNavComponent } from './Components/admin/admin-nav/admin-nav.component';
import { AdminUsersComponent } from './Components/admin/admin-users/admin-users.component';
import { AdminProductsComponent } from './Components/admin/admin-products/admin-products.component';
import { AdminOrdersComponent } from './Components/admin/admin-orders/admin-orders.component';
import { AdminCatalogComponent } from './Components/admin/admin-catalog/admin-catalog.component';
import { AdminStatsComponent } from './Components/admin/admin-stats/admin-stats.component';
import { AdminMessagesComponent } from './Components/admin/admin-messages/admin-messages.component';
import { AdminNewsletterComponent } from './Components/admin/admin-newsletter/admin-newsletter.component';
import { AdminCouponsComponent } from './Components/admin/admin-coupons/admin-coupons.component';
import { AdminPaymentMethodsComponent } from './Components/admin/admin-payment-methods/admin-payment-methods.component';
import { AdminPromotionsComponent } from './Components/admin/admin-promotions/admin-promotions.component';
import { ToastComponent } from './Components/toast/toast.component';
import { ForgotPasswordComponent } from './Components/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './Components/reset-password/reset-password.component';
import { ConfirmModalComponent } from './Components/confirm-modal/confirm-modal.component';
import { OrderConfirmationComponent } from './Components/order-confirmation/order-confirmation.component';
import { JwtModule } from '@auth0/angular-jwt';
import { SharedModule } from './Module/shared/shared.module';
import { ProductCardComponent } from './Components/product-card/product-card.component';
import { CarouselComponent } from './Components/carousel/carousel.component';
import { HeroComponent } from './Components/hero/hero.component';
import { AdminHeroComponent } from './Components/admin/admin-hero/admin-hero.component';
import { AdminAuditLogsComponent } from './Components/admin/admin-audit-logs/admin-audit-logs.component';

@NgModule({
  declarations: [
    AppComponent,
    NavigationComponent,
    FooterComponent,
    HomeComponent,
    ShopComponent,
    SingleProductComponent,
    CartComponent,
    ContactComponent,
    CheckoutComponent,
    AboutUsComponent,
    LoginComponent,
    RegisterComponent,
    ProfileComponent,
    AdminNavComponent,
    AdminUsersComponent,
    AdminProductsComponent,
    AdminOrdersComponent,
    AdminCatalogComponent,
    AdminStatsComponent,
    AdminMessagesComponent,
    AdminNewsletterComponent,
    AdminCouponsComponent,
    AdminPaymentMethodsComponent,
    AdminPromotionsComponent,
    ToastComponent,
    ForgotPasswordComponent,
    ResetPasswordComponent,
    ConfirmModalComponent,
    OrderConfirmationComponent,
    ProductCardComponent,
    CarouselComponent,
    HeroComponent,
    AdminHeroComponent,
    AdminAuditLogsComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    SharedModule,
    ReactiveFormsModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: () => {
          return localStorage.getItem('token');
        },
        allowedDomains: ['http://localhost:4200'],
        disallowedRoutes: ['http://localhost:5277/api/token'],
      },
    }),
  ],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}
