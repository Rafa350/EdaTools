<?xml version="1.0"?>
<!-- Genera 'centroids' per EURO-CIRCUITS -->
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:output method="text" encoding="windows-1252"/>

    <xsl:template match="/">
        <xsl:text>reference designator, x, y, rotate, side</xsl:text>
        <xsl:text>&#13;&#10;</xsl:text>
        <xsl:apply-templates select="board/parts"/>
    </xsl:template>

    <xsl:template match="part">
        <xsl:if test="attributes/attribute[@name='REFERENCE'] and (not (attributes/attribute[@name='NP']))">
            <xsl:value-of select="@name"/>
            <xsl:text>, </xsl:text>
            <xsl:value-of select="@position"/>
            <xsl:text>, </xsl:text>
            <xsl:choose>
                <xsl:when test="@rotation">
                    <xsl:value-of select="@rotation"/>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:text>0</xsl:text>
                </xsl:otherwise>
            </xsl:choose>
            <xsl:text>, </xsl:text>
            <xsl:choose>
                <xsl:when test="@side">
                    <xsl:value-of select="@side"/>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:text>top</xsl:text>
                </xsl:otherwise>
            </xsl:choose>
            <xsl:text>&#13;&#10;</xsl:text>
        </xsl:if>
    </xsl:template>

</xsl:stylesheet>