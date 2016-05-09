<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd" doctype-public="-//W3C//DTD XHTML 1.0 Strict//EN" indent="yes" standalone="yes"  />
  <xsl:decimal-format name="usd" decimal-separator="." grouping-separator=","/>
  <xsl:decimal-format name="qty" decimal-separator="." grouping-separator=","/>
  <xsl:template match="Invoice">
    <xsl:variable name="DocType">
      <xsl:choose>
        <xsl:when test="IsEstimate = 'true'">Estimate</xsl:when>
        <xsl:otherwise>Invoice</xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <html>
      <head>
        <title>
          <xsl:value-of select="$DocType"/> - <xsl:value-of select="Number"/>
        </title>
        <style type="text/css" media="print, screen">
          body
          {
          font-family: Tahoma;
          font-size: 14px;
          }
          #invoiceContainer
          {
          width: 900px;
          margin: 0 auto;
          padding: 10px;
          }
          h2
          {
          font-weight: bold;
          margin: 0;
          font-size: 170%;
          }
          h3
          {
          font-weight: bold;
          margin: 0;
          font-size: 150%;
          }
          h4
          {
          font-weight: bold;
          margin: 0;
          font-size: 130%;
          }
          .right
          {
          float: right;
          text-align: right;
          }
          .left
          {
          clear: both;
          float: left;
          margin-bottom: 20px;
          }
          .header
          {
          clear: left;
          float: left;
          width: 100%;
          }
          .fromto
          {
          clear: left;
          float: left;
          width: 100%;
          }
          .imgWrapper
          {
          text-align:center;
          }
          dt
          {
          font-size: 120%;
          font-weight: bold;
          margin-bottom: 5px;
          }
          .bottomSep
          {
          border-bottom: 3px solid #000000;
          padding-bottom:5px;
          }
          .lines table,
          .payments table
          {
          width:100%;
          padding:3px;
          border:none;
          border-collapse:collapse;
          margin:20px auto;
          }
          .lines table th
          {
          text-align:left;
          }
          .lines table th.num
          {
          text-align:right;
          }
          .num
          {
          text-align:right;
          width:80px;
          padding-right:20px;
          white-space:nowrap;
          }
          .alt
          {
          background-color:#d9d9d9;
          }
          .summary .num
          {
          font-weight:bold;
          background-color:#c0c0c0;
          }
          .heading
          {
          font-weight:bold;
          }
          #jobLocAddress
          {
          float:left;
          padding-left: 100px;
          }
          .sigs img
          {
          width:300px;
          }
          .sigs {text-align:center;}
          .sigs div {width:400px;display:inline-block;}
        </style>
      </head>
      <body>
        <div id="invoiceContainer">
          <div class="header">
            <div class="left">
              <h2>
                <xsl:value-of select="$DocType"/>
              </h2>
              <h3>
                #<xsl:value-of select="JobID"/>
              </h3>
            </div>
            <div class="right">
              <h4>
                <xsl:value-of select="$DocType"/> Date:
                <xsl:call-template name="formatDate">
                  <xsl:with-param name="dateTime" select="Date"/>
                </xsl:call-template>
              </h4>
              <h4>
                Service Date:
                <xsl:call-template name="formatDate">
                  <xsl:with-param name="dateTime" select="Completed"/>
                </xsl:call-template>
              </h4>
              <h4>
                Technician: <xsl:value-of select="Technician"/>
              </h4>
            </div>
          </div>
          <div class="fromto bottomSep">
            <div class="imgWrapper">
              <img>
                <xsl:choose>
                  <xsl:when test="FranchiseTypeID = 6 and DBAId = 0">
                    <xsl:attribute name="src">
                      http://conn951.gearhost.us.com/fileutils/franchiseimages/<xsl:value-of select="FranchiseId"/>_dbaimage.jpg
                    </xsl:attribute>
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:attribute name="src">
                      http://conn951.gearhost.us.com/fileutils/dbaimages/<xsl:value-of select="DBAId"/>_dbaimage.jpg
                    </xsl:attribute>
                  </xsl:otherwise>
                </xsl:choose>

              </img>
            </div>
            <div class="left">
              <xsl:value-of select="FromName"/><br />
              <xsl:value-of select="FromAddress"/><br />
              <xsl:value-of select="FromCity"/>, <xsl:value-of select="FromState"/>&#160;<xsl:value-of select="FromZip"/>
            </div>
            <div class="right">
              <xsl:value-of select="LicenseNumber"/>
              <br />
              <xsl:value-of select="FromPhone"/>
            </div>
            <div class="left">
              <span class="heading">Billing Address</span><br />
              <xsl:value-of select="ToName"/><br />
              <xsl:value-of select="ToAddress"/><br />
              <xsl:value-of select="ToCity"/>, <xsl:value-of select="ToState"/>&#160;<xsl:value-of select="ToZip"/>
            </div>
            <div id="jobLocAddress">
              <span class="heading">Job Address</span><br />
              <xsl:value-of select="LocationName"/><br />
              <xsl:value-of select="LocationAddress"/><br />
              <xsl:value-of select="LocationCity"/>, <xsl:value-of select="LocationState"/>&#160;<xsl:value-of select="LocationZip"/>
            </div>
          </div>
          <xsl:if test="Diagnostics != '' or Recommendations != ''">

            <dl class="bottomSep">
              <xsl:if test="Diagnostics != ''">
                <dt>Diagnostic Notes</dt>
                <dd>
                  <xsl:value-of select="Diagnostics"/>
                </dd>
              </xsl:if>
              <xsl:if test="Recommendations != ''">
                <dt>Recommendations</dt>
                <dd>
                  <xsl:value-of select="Recommendations"/>
                </dd>
              </xsl:if>
            </dl>
          </xsl:if>
          <div class="lines bottomSep">
            <h4>Charges</h4>
            <table>
              <tr>
                <th class="num">Quantity</th>
                <th>Description</th>
                <th class="num">Unit Price</th>
                <th class="num">Total Price</th>
              </tr>
              <xsl:for-each select="Lines/InvoiceLine">

                <tr>
                  <xsl:choose>
                    <xsl:when test="position() mod 2 = 0">
                      <xsl:attribute name="class">alt</xsl:attribute>
                    </xsl:when>
                    <xsl:otherwise>
                      <xsl:text></xsl:text>
                    </xsl:otherwise>
                  </xsl:choose>
                  <td class="num">
                    <xsl:value-of select="format-number(Quantity, '#,##0.00', 'usd')"/>
                  </td>
                  <td>
                    <xsl:value-of select="Description"/>
                  </td>
                  <td class="num">
                    <xsl:value-of select="format-number(UnitPrice, '$#,##0.00', 'usd')"/>
                  </td>
                  <td class="num">
                    <xsl:value-of select="format-number(ExtendedPrice, '$#,##0.00', 'usd')"/>
                  </td>
                </tr>
              </xsl:for-each>
              <tr class="summary">
                <td />
                <td />
                <td class="num">Subtotal</td>
                <td class="num">
                  <xsl:value-of select="format-number(SubTotal, '$#,##0.00', 'usd')"/>
                </td>
              </tr>
              <tr class="summary">
                <td />
                <td />
                <td class="num">Tax</td>
                <td class="num">
                  <xsl:value-of select="format-number(TaxAmount, '$#,##0.00', 'usd')"/>
                </td>
              </tr>
              <tr class="summary">
                <td />
                <td />
                <td class="num">Total</td>
                <td class="num">
                  <xsl:value-of select="format-number(TotalAmount, '$#,##0.00', 'usd')"/>
                </td>
              </tr>
            </table>
          </div>
          <xsl:if test="HasAuthSignature = 'true' or HasAcceptSignature = 'true'">
            <div class="sigs bottomSep">
              <xsl:if test="HasAuthSignature = 'true'">
                <div>
                  <h4>Authorized By</h4>
                  <img>
                    <xsl:attribute name="src">
                      http://demo.connectuspro.net/OwnerPortal/Signature/Get/<xsl:value-of select="JobID" />?PictureID=0
                    </xsl:attribute>
                  </img>
                </div>
              </xsl:if>
              <xsl:if test="HasAcceptSignature = 'true'">
                <div>
                  <h4>Accepted By</h4>
                  <img>
                    <xsl:attribute name="src">
                      http://demo.connectuspro.net/OwnerPortal/Signature/Get/<xsl:value-of select="JobID" />?PictureID=1
                    </xsl:attribute>
                  </img>
                </div>
              </xsl:if>
            </div>
          </xsl:if>
          <div class="payments bottomSep">
            <h4>Payments</h4>
            <table>
              <xsl:for-each select="Payments/Payment">
                <tr>
                  <xsl:choose>
                    <xsl:when test="position() mod 2 = 0">
                      <xsl:attribute name="class">alt</xsl:attribute>
                    </xsl:when>
                    <xsl:otherwise>
                      <xsl:text></xsl:text>
                    </xsl:otherwise>
                  </xsl:choose>
                  <td>
                    <xsl:value-of select="PaymentType"/>
                  </td>
                  <td>
                    <xsl:value-of select="Description"/>
                  </td>
                  <td class="num">
                    <xsl:value-of select="format-number(Amount, '$#,##0.00', 'usd')"/>
                  </td>
                </tr>
              </xsl:for-each>
              <tr class="summary">
                <td></td>
                <td class="num">Balance Due</td>
                <td class="num">
                  <xsl:value-of select="format-number(Balance, '$#,##0.00', 'usd')"/>
                </td>
              </tr>
            </table>
          </div>
          <xsl:if test="Warranty1Length != '' or Warranty2Length != ''">
            <div class="warranty bottomSep">
              <h4>Warranty</h4>
              <xsl:if test="Warranty1Length != ''">
                <p>
                  <xsl:value-of select="Warranty1Length"/> - <xsl:value-of select="Warranty1"/>
                </p>
              </xsl:if>
              <xsl:if test="Warranty2Length != ''">
                <p>
                  <xsl:value-of select="Warranty2Length"/> - <xsl:value-of select="Warranty2"/>
                </p>
              </xsl:if>
            </div>
          </xsl:if>
        </div>
      </body>
    </html>
  </xsl:template>
  <xsl:template name="formatDate">
    <xsl:param name="dateTime" />
    <xsl:variable name="date" select="substring-before($dateTime, 'T')" />
    <xsl:variable name="year" select="substring-before($date, '-')" />
    <xsl:variable name="month" select="substring-before(substring-after($date, '-'), '-')" />
    <xsl:variable name="day" select="substring-after(substring-after($date, '-'), '-')" />
    <xsl:value-of select="concat($month, '/', $day, '/', $year)" />
  </xsl:template>

  <xsl:template name="formatTime">
    <xsl:param name="dateTime" />
    <xsl:value-of select="substring-after($dateTime, 'T')" />
  </xsl:template>

</xsl:stylesheet>
