import { Component, HostListener } from '@angular/core';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.scss'],
})
export class FooterComponent {
  showBackToTop: boolean = false;

  @HostListener('window:scroll')
  onWindowScroll(): void {
    this.showBackToTop = window.scrollY > 100;
  }

  scrollToTop() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
}
