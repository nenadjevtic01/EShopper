import { DetachedRouteHandle, RouteReuseStrategy } from '@angular/router';

export class CustomRouteReuseStrategy implements RouteReuseStrategy {
  retrieve(): DetachedRouteHandle | null {
    return null;
  }

  shouldAttach(): boolean {
    return false;
  }

  shouldDetach(): boolean {
    return false;
  }

  shouldReuseRoute(): boolean {
    return false;
  }

  store(): void {}
}
