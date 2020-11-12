import { Directive, ElementRef, Input, AfterViewInit } from '@angular/core';
import 'jquery-slimscroll/jquery.slimscroll.min.js';

// tslint:disable-next-line:directive-selector
@Directive({ selector: '[mySlimScroll]' })

export class SlimScrollDirective implements AfterViewInit {
  el: ElementRef;
  constructor(el: ElementRef) {
    this.el = el;
  }

  // tslint:disable-next-line:member-ordering
  @Input() scrollHeight: string;

  ngAfterViewInit() {
    const $el = $(this.el.nativeElement);

    ($el as any).slimScroll({
      height: this.scrollHeight || '100%'
    });
  }
}
