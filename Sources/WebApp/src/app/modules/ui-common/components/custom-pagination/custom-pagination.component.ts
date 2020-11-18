import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {PageEvent} from '@angular/material';

@Component({
  selector: 'app-custom-pagination',
  templateUrl: './custom-pagination.component.html',
  styleUrls: ['./custom-pagination.component.sass']
})
export class CustomPaginationComponent implements OnInit {
  @Input() pageIndex: number;
  @Input() pageLength: number;
  @Input() dataLength: number;
  @Input() pageSizeOptions: number[] = [20, 25, 30];
  @Output() pageChangeEvent = new EventEmitter<PaginationParams>();

  constructor() {
  }

  ngOnInit() {
  }

  onChangePage($event: PageEvent) {
    this.pageLength = $event.pageSize;
    this.pageIndex = $event.pageIndex;
    this.pageChangeEvent.emit({pageLength: this.pageLength, pageIndex: this.pageIndex});
  }

}

export class PaginationParams {
  pageIndex: number;
  pageLength: number;
}
