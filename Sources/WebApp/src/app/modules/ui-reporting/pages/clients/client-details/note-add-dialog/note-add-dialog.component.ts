import {Component, Inject, OnInit} from '@angular/core';
import {FormBuilder, FormGroup} from '@angular/forms';
import {TradeAccountNoteModel} from '../note/trade-account-note.model';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {ClientsService} from '../../clients.service';
import {catchError, finalize} from 'rxjs/operators';
import {of} from 'rxjs';
import {NoteColor} from '../note/note-color.enum';
import {LoadingService} from '../../../../../core/services/loading.service';

@Component({
  selector: 'app-note-add-dialog',
  templateUrl: './note-add-dialog.component.html',
  styleUrls: ['./note-add-dialog.component.sass']
})
export class NoteAddDialogComponent implements OnInit {

  form: FormGroup;
  tradeNote: TradeAccountNoteModel = new TradeAccountNoteModel();
  color: NoteColor = NoteColor.ORANGE;
  colorEnum = NoteColor;


  constructor(private fBuilder: FormBuilder,
              private dialogRef: MatDialogRef<NoteAddDialogComponent>,
              private clientsService: ClientsService,
              private loading: LoadingService,
              @Inject(MAT_DIALOG_DATA) private tradeAccountId: number) {
  }

  ngOnInit() {
    this.tradeNote.tradeAccountId = this.tradeAccountId;
    this.tradeNote.text = '';

    this.initForm();
  }

  private initForm() {
    this.form = this.fBuilder.group({
      text: [this.tradeNote.text]
    });
  }

  save() {
    this.loading.setLoading(true);
    this.clientsService.addNewTradeAccountNote(this.tradeNote)
      .pipe(
        catchError(() => {
          return of(-1);
        }),
        finalize(() => this.loading.setLoading(false))
      )
      .subscribe(data => {
        if (data === -1) {
          this.dialogRef.close(false);
        } else {
          this.dialogRef.close(true);
        }
      });
  }

  cancel() {
    this.dialogRef.close(false);
  }

  changeColor(color: NoteColor) {
    this.color = color;
    this.tradeNote.noteType = color;
  }
}
