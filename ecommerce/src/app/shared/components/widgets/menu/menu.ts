import { AsyncPipe, DatePipe, NgClass, NgTemplateOutlet } from '@angular/common';
import { Component, inject, input } from '@angular/core';
import { Router, RouterModule } from '@angular/router';

import { TranslateModule } from '@ngx-translate/core';
import { Store } from '@ngxs/store';
import { Observable } from 'rxjs';

import { environment } from '../../../../../environments/environment';
import { IBlog } from '../../../interface/blog.interface';
import { IMenuModel } from '../../../interface/menu.interface';
import { IProduct } from '../../../interface/product.interface';
import { MenuService } from '../../../services/menu.service';
import { GetSelectedBlogsAction } from '../../../store/action/blog.action';
import { GetMenuProductsAction } from '../../../store/action/product.action';
import { BlogState } from '../../../store/state/blog.state';
import { MenuState } from '../../../store/state/menu.state';
import { ProductState } from '../../../store/state/product.state';
import { IMenu } from '../../interface/menu.interface';
import { NoData } from '../no-data/no-data';
import { ProductBox } from '../product-box/product-box';
import { LinkBox } from './link-box/link-box';

@Component({
  selector: 'app-menu',
  imports: [
    RouterModule,
    TranslateModule,
    LinkBox,
    NoData,
    ProductBox,
    AsyncPipe,
    DatePipe,
    NgClass,
    NgTemplateOutlet,
  ],
  templateUrl: './menu.html',
  styleUrl: './menu.scss',
})
export class Menu {
  private store = inject(Store);
  private router = inject(Router);
  menuService = inject(MenuService);

  readonly class = input<string>();

  blog$: Observable<IBlog[]> = inject(Store).select(BlogState.selectedBlogs);
  menu$: Observable<IMenuModel> = inject(Store).select(MenuState.menu);
  menuProduct$: Observable<IProduct[]> = inject(Store).select(ProductState.menuProducts);

  public menu: IMenu[] = [];
  public products: IProduct[];
  public blogs: IBlog[];

  public StorageURL = environment.storageURL;

  constructor() {
    this.menu$.subscribe(menu => {
      const productIds = Array.from(new Set(this.concatDynamicProductKeys(menu, 'product_ids')));
      if (productIds && productIds.length) {
        this.store.dispatch(new GetMenuProductsAction({ ids: productIds?.join() }));

        this.menuProduct$.subscribe(products => {
          this.products = products.slice(0, 2);
        });
      }

      const blogIds = Array.from(new Set(this.concatDynamicProductKeys(menu, 'blog_ids')));
      if (blogIds && blogIds.length) {
        this.store.dispatch(new GetSelectedBlogsAction({ status: 1, ids: blogIds?.join() }));
        this.blog$.subscribe(blog => {
          this.blogs = blog.slice(0, 2);
        });
      }
    });
  }

  mainMenuOpen() {
    this.menuService.mainMenuToggle = true;
  }

  mainMenuClose() {
    this.menuService.mainMenuToggle = false;
  }

  redirect(path: string) {
    void this.router.navigateByUrl(path);
  }

  toggle(menu: IMenu) {
    if (!menu.active) {
      this.menu.forEach(item => {
        if (this.menu.includes(menu)) {
          item.active = false;
        }
      });
    }
    menu.active = !menu.active;
  }

  concatDynamicProductKeys(obj: IMenu, keyName: 'product_ids' | 'blog_ids'): number[] {
    const result: number[] = [];

    function traverse(value: unknown): void {
      if (Array.isArray(value)) {
        value.forEach(traverse);
      } else if (typeof value === 'object' && value !== null) {
        const record = value as Record<string, unknown>;
        for (const key in record) {
          const prop = record[key];

          if (key === keyName && Array.isArray(prop)) {
            result.push(...(prop as number[]));
          } else {
            traverse(prop);
          }
        }
      }
    }

    traverse(obj);
    return result;
  }
}
