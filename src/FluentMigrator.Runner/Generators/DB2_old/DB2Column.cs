using System;
using System.Collections.Generic;
using System.Linq;
using FluentMigrator.Model;
using FluentMigrator.Runner.Generators.Base;

namespace FluentMigrator.Runner.Generators.DB2
{
    internal class DB2Column : ColumnBase
    {
    	private const int  DB2ConstraintNameMaxLength = 18;
    	private const int DB2ObjectNameMaxLength = 30;

		public DB2Column()
            : base(new DB2TypeMap(), new DB2Quoter())
        {
        }

        protected override string FormatIdentity(ColumnDefinition column)
        {
            if (column.IsIdentity)
            {
                throw new DatabaseOperationNotSupportedException("DB2 does not support identity columns. Please use a SEQUENCE instead");
            }
            return string.Empty;
        }

        protected override string FormatSystemMethods(SystemMethods systemMethod)
        {
            switch (systemMethod)
            {
                case SystemMethods.NewGuid:
					return "GENERATE_UNIQUE()";
            }

            return null;
        }

        protected override string GetPrimaryKeyConstraintName(IEnumerable<ColumnDefinition> primaryKeyColumns, string tableName)
        {
            if (primaryKeyColumns == null)
                throw new ArgumentNullException("primaryKeyColumns");
            if (tableName == null)
                throw new ArgumentNullException("tableName");

            var primaryKeyName = primaryKeyColumns.First().PrimaryKeyName;

            if (string.IsNullOrEmpty(primaryKeyName))
            {
                return string.Empty;
            }

			//if (primaryKeyName.Length > DB2ConstraintNameMaxLength)
			//    throw new DatabaseOperationNotSupportedException(
			//        string.Format(
			//            "DB2 does not support length of primary key name greater than {0} characters. Reduce length of primary key name. ({1})",
			//            DB2ObjectNameMaxLength, primaryKeyName));

            var result = string.Format("CONSTRAINT {0} ", Quoter.QuoteIndexName(primaryKeyName));
            return result;
        }
    }
}