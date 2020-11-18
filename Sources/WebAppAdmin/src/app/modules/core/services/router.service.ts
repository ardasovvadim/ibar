import { Injectable } from '@angular/core';
import {Router} from '@angular/router';

@Injectable()
export class RouterService {

  constructor(private router: Router) { }

  navigate(path: string = '/') {
    this.router.navigate([path]);
  }

}
