#region License
// 
// Copyright (c) 2007-2009, Sean Chambers <schambers80@gmail.com>
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System.Data;
using FluentMigrator.Runner.Generators.Base;

namespace FluentMigrator.Runner.Generators.DB2
{
    internal class DB2TypeMap : TypeMapBase
    {
        public const int AnsiStringCapacity = 2000;
        public const int AnsiTextCapacity = 2147483647;
        public const int UnicodeStringCapacity = 254;
        public const int UnicodeTextCapacity = int.MaxValue;
        public const int BlobCapacity = 2147483647;
        public const int DecimalCapacity = 19;
        public const int XmlCapacity = 1073741823;

        protected override void SetupTypeMaps()
        {
            SetTypeMap(DbType.AnsiStringFixedLength, "CHAR(254)");
            SetTypeMap(DbType.AnsiStringFixedLength, "CHAR($size)", AnsiStringCapacity);
            SetTypeMap(DbType.AnsiString, "VARCHAR(254)");
            SetTypeMap(DbType.AnsiString, "VARCHAR($size)", AnsiStringCapacity);
            SetTypeMap(DbType.AnsiString, "CLOB", AnsiTextCapacity);
            SetTypeMap(DbType.Binary, "BLOB", BlobCapacity);
			SetTypeMap(DbType.Boolean, "SMALLINT");
			SetTypeMap(DbType.Byte, "SMALLINT");
			SetTypeMap(DbType.Currency, "DECIMAL(16,4)");
            SetTypeMap(DbType.Date, "DATE");
            SetTypeMap(DbType.DateTime, "TIMESTAMP");
            SetTypeMap(DbType.Decimal, "DECIMAL(19,5)");
            SetTypeMap(DbType.Decimal, "NUMBER($size,$precision)", DecimalCapacity);
            SetTypeMap(DbType.Double, "DOUBLE");
			SetTypeMap(DbType.Guid, "CHARACTER(16) FOR BIT DATA");
            SetTypeMap(DbType.Int16, "SMALLINT");
            SetTypeMap(DbType.Int32, "INTEGER");
            SetTypeMap(DbType.Int64, "BIGINT");
            SetTypeMap(DbType.Single, "REAL");
            SetTypeMap(DbType.StringFixedLength, "CHAR(254)");
            SetTypeMap(DbType.StringFixedLength, "CHAR($size)", UnicodeStringCapacity);
            SetTypeMap(DbType.String, "VARCHAR(254)");
            SetTypeMap(DbType.String, "VARCHAR($size)", 8000);
			SetTypeMap(DbType.String, "CLOB", BlobCapacity);
            SetTypeMap(DbType.Time, "TIME");
            SetTypeMap(DbType.Xml, "XML");
        }
    }
}