import {FileState} from './file-state.enum';
import {FileStatus} from './file-status.enum';

export class SourceDataModel {
  data: FileModel[];
  dataLength: number;
}

export class FileModel {
  id: number;
  fileName: string;
  dateOfFile: Date;
  dateOfTheProcessing: Date;
  fileState: FileState;
  fileStatus: FileStatus;
  isForApi: boolean;
  isSent: boolean;
  exception: string;
}
