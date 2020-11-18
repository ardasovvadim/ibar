  USE [IBAR.Master]
  GO

  UPDATE  [FileNameRegexes] 
  SET FileRegex = '[A-Za-z]{1}\d{6,7}\.[A-Za-z]{1}\d{6,7}-Sytoss-clientfees-det\.\d{8}\.\d{8}'
  WHERE FileName = 'ClientFeesRegex'
  
  UPDATE  [FileNameRegexes] 
  SET FileRegex = 'acct_status_report_non_eca\.[A-Za-z]{1}\d{6,7}\.\d{8}'
  WHERE FileName = 'AcctStatusReportNonEcaRegex'
  
  UPDATE  [FileNameRegexes] 
  SET FileRegex = 'acct_status_report\.[A-Za-z]{1}\d{6,7}\.\d{8}'
  WHERE FileName = 'AcctStatusReportRegex'

  UPDATE  [FileNameRegexes] 
  SET FileRegex = '[A-Za-z]{1}\d{6,7}\.[A-Za-z]{1}\d{6,7}-Sytoss-Cash\.\d{8}\.\d{8}'
  WHERE FileName = 'SytossCashReportRegex'

  UPDATE  [FileNameRegexes] 
  SET FileRegex = '[A-Za-z]{1}\d{6,7}\.[A-Za-z]{1}\d{6,7}-Sytoss-Cash\.\d{8}\.\d{8}'
  WHERE FileName = 'SytossNavRegex'

  
  UPDATE  [FileNameRegexes] 
  SET FileRegex = '[A-Za-z]{1}\d{6,7}\.[A-Za-z]{1}\d{6,7}-Sytoss-Nav\.\d{8}\.\d{8}'
  WHERE FileName = 'SytossTradesAsRegex'

  UPDATE  [FileNameRegexes] 
  SET FileRegex = '[A-Za-z]{1}\d{6,7}\.[A-Za-z]{1}\d{6,7}-Sytoss-Trades-ass\.\d{8}\.\d{8}'
  WHERE FileName = 'SytossClientInfoRegex'

      UPDATE  [FileNameRegexes] 
  SET FileRegex = '[A-Za-z]{1}\d{6,7}\.[A-Za-z]{1}\d{6,7}-Sytoss-Open_Positions\.\d{8}\.\d{8}'
  WHERE FileName = 'SytossOpenPositions'


    UPDATE  [FileNameRegexes] 
  SET FileRegex = '[A-Za-z]{1}\d{6,7}\.[A-Za-z]{1}\d{6,7}-Sytoss-Trades-EXE\.\d{8}\.\d{8}'
  WHERE FileName = 'SytossTradesEXE'
  
    UPDATE  [FileNameRegexes] 
  SET FileRegex = '[A-Za-z]{1}\d{6,7}\.[A-Za-z]{1}\d{6,7}-Sytoss-InterestAccrua\.\d{8}\.\d{8}'
  WHERE FileName = 'SytossInterestAccrua'

    UPDATE  [FileNameRegexes] 
  SET FileRegex = '[A-Za-z]{1}\d{6,7}\.[A-Za-z]{1}\d{6,7}-Sytoss-Commission_Det\.\d{8}\.\d{8}'
  WHERE FileName = 'SytossCommissionsDet'

      UPDATE  [FileNameRegexes] 
  SET FileRegex = '[A-Za-z]{1}\d{6,7}\.Commissions_CRM'
  WHERE FileName = 'CommissionsCrmRegex'

      UPDATE  [FileNameRegexes] 
  SET FileRegex = '[A-Za-z]{1}\d{6,7}\.All_kinds_of_data_CRM'
  WHERE FileName = 'AllKindsOfDataCrmRegex'

      UPDATE  [FileNameRegexes] 
  SET FileRegex = '[A-Za-z]{1}\d{6,7}\.Ending_value_CRM'
  WHERE FileName = 'EndingValueCrmRegex'
