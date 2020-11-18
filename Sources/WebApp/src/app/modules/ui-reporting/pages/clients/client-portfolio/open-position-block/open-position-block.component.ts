import {Component, Input, OnInit} from '@angular/core';
import {OpenPositionModel} from '../open-position.model';

@Component({
  selector: 'app-open-position-block',
  templateUrl: './open-position-block.component.html',
  styleUrls: ['./open-position-block.component.sass']
})
export class OpenPositionBlockComponent implements OnInit {

  @Input() openPosition: OpenPositionModel = new OpenPositionModel();
  @Input() index = 0;

  constructor() { }

  ngOnInit() {
  }

}
