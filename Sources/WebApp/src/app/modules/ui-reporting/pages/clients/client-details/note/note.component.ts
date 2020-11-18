import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {TradeAccountNoteModel} from './trade-account-note.model';
import {NoteColor} from './note-color.enum';
import {MatDialog} from '@angular/material/dialog';

@Component({
  selector: 'app-note',
  templateUrl: './note.component.html',
  styleUrls: ['./note.component.sass']
})
export class NoteComponent implements OnInit {

  @Input() tradeNote: TradeAccountNoteModel;
  @Input() color = NoteColor.ORANGE;
  @Input() isReadonly = true;
  @Input() isControls = true;
  @Input() isAuditBlock = true;
  @Output() deleteEvent = new EventEmitter<TradeAccountNoteModel>();

  colorEnum = NoteColor;

  constructor(private dialog: MatDialog) {
  }

  ngOnInit() {
    if (typeof this.tradeNote.text === 'undefined' || this.tradeNote.text == null) {
      this.tradeNote.text = '';
    }
    const noteType = this.tradeNote.noteType;
    switch (noteType) {
      case 1:
        this.color = NoteColor.YELLOW;
        break;
      case 0:
      default:
        this.color = NoteColor.ORANGE;
        break;
    }
  }

  delete() {
    this.deleteEvent.emit(this.tradeNote);
  }

}
